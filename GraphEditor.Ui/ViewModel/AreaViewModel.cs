using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Container;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Utils;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class AreaViewModel : BaseNotification
    {
        private struct ConnectingNodeData
        {
            internal NodeViewModel SourceNode;
            internal int ConnIdx;
            internal bool IsOutBound;
        }

        ConnectingNodeData _connNodeData;
        private UserControl _nodeConfigUi;

        public ObservableCollection<NodeViewModel> NodeVMs { get; set; }

        public RelayCommand LoadCommand { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand AddNodeCommand { get; private set; }

        public void OnBuiltUp()
        {
            LoadCommand = new RelayCommand(o => LoadExec());
            SaveCommand = new RelayCommand(o => SaveExec());
            AddNodeCommand = new RelayCommand(o => AddNodeExec((INodeTypeData) o));

            ToolBar = new ToolBarViewModel(LoadCommand, SaveCommand);
            AreaContextMenuItems = new ObservableCollection<INodeTypeData>();
            NodeVMs = new ObservableCollection<NodeViewModel>();

            ServiceContainer.Get<INodeTypeRepository>().NodeTypes.ForEach(
                nt => AreaContextMenuItems.Add(nt)
            );

            UiMessageHub.OnRemoveNode += OnRemoveNode;
            UiMessageHub.OnConnectRequested += OnConnectRequested;
            UiMessageHub.OnCreateConnection += OnCreateConnection;
        }

        public void OnTearDown()
        {
            ClearConfig();
        }

        private void ClearConfig()
        {
            NodeVMs.InverseFor((node, i) => node.RemoveExec());
        }

        private void LoadExec()
        {
            //var dlg = new OpenFileDialog();
            //if (dlg.ShowDialog() == true)
            //{
            ClearConfig();

            var docXml = XDocument.Load("c:\\gn.xml");
            var configXml = docXml.Element("Configuration");
            var nodesXml = configXml.Element("Nodes");
            nodesXml.Elements().ForEach(node =>
            {
                var nodeVm = AddNodeExec(ServiceContainer.Get<INodeTypeRepository>().Find(node.Attribute("Type").Value));
                var pts = node.Attribute("Location").Value.Split(';');
                nodeVm.Location = Point.Parse(string.Join(",", pts));

                nodeVm.LoadNodeFromXml(node);
            });


            // this is a workaround to ensure that the nodes are loaded and all bound item collections initialized before the connections are loaded
            Task.Run(() =>
            {
                Thread.Sleep(20);  // important!

                var connectionsXml = configXml.Element("Connections");
                connectionsXml.Elements().ForEach(conn =>
                {
                    var sourceId = conn.Attribute("Source").Value;
                    var sourceConn = int.Parse(conn.Attribute("SourceConn").Value);
                    var targetId = conn.Attribute("Target").Value;
                    var targetConn = int.Parse(conn.Attribute("TargetConn").Value);

                    _connNodeData.SourceNode = NodeVMs.First(node => node.Data.Id == sourceId);
                    _connNodeData.ConnIdx = sourceConn;
                    _connNodeData.IsOutBound = true;

                    var targetNode = NodeVMs.First(node => node.Data.Id == targetId);

                    CurrentDispatcher.Invoke(() => OnCreateConnection(targetNode, targetConn));
                });

                _connNodeData.SourceNode = null;
            });
            //}
        }
        private void SaveExec()
        {
            //var dlg = new SaveFileDialog();
            //if (dlg.ShowDialog() == true)
            //{
            var docXml = new XDocument();

            var configXml = new XElement("Configuration");

            var nodesXml = new XElement("Nodes");
            NodeVMs.ForEach(node => node.SaveNodeToXml(nodesXml));
            configXml.Add(nodesXml);

            var connXml = new XElement("Connections");
            NodeVMs.ForEach(node => node.SaveConnectionsToXml(connXml));
            configXml.Add(connXml);

            docXml.Add(configXml);

            docXml.Save("c:\\gn.xml");
            //}
        }

        // called by IoC container
        public void ShutDown()
        {
            UiMessageHub.Dispose();
        }

        public ToolBarViewModel ToolBar { get; private set; }

        public ObservableCollection<INodeTypeData> AreaContextMenuItems { get; private set; }

        // TODO: Raster-Breite in UI editierbar machen
        public Rect GridRect { get; } = new Rect { X = 0, Y = 0, Width = UiConst.GridWidth, Height = UiConst.GridWidth };

        public NodeViewModel AddNodeExec(INodeTypeData nodeTypeData)
        {
            var newNodeVm = new NodeViewModel(nodeTypeData, () => NodeVMs.ToList(), OnOpenConfigUi);
            NodeVMs.Add(newNodeVm);

            UiMessageHub.AddNode(newNodeVm);

            return newNodeVm;
        }

        public UserControl NodeConfigUi
        {
            get => _nodeConfigUi;
            private set => SetProperty<AreaViewModel, UserControl>(ref _nodeConfigUi, value, nameof(NodeConfigUi));
        }

        private void OnOpenConfigUi(INodeConfigUi configUi)
        {
            configUi.OnClose += ui => NodeConfigUi = null;
            NodeConfigUi = configUi as UserControl;
        }

        public void OnRemoveNode(NodeViewModel nodeVm)
        {
            NodeVMs.Remove(nodeVm);
        }

        public void RevokeConnectRequestStatus()
        {
            if (_connNodeData.SourceNode != null)
            {
                if (_connNodeData.IsOutBound)
                    _connNodeData.SourceNode.OutConnectors[_connNodeData.ConnIdx].IsConnecting = false;
                else
                    _connNodeData.SourceNode.InConnectors[_connNodeData.ConnIdx].IsConnecting = false;
            }
        }

        private void UpdateConnectingNodeData(bool isConnecting, NodeViewModel sourceNode, int connIdx, bool isOutBound)
        {
            if (isConnecting)
            {
                _connNodeData.SourceNode = sourceNode;
                _connNodeData.ConnIdx = connIdx;
                _connNodeData.IsOutBound = isOutBound;
            }
            else
                _connNodeData.SourceNode = null;
        }

        private void OnConnectRequested(bool isConnecting, NodeViewModel sourceNode, int connIdx, bool isOutBound)
        {
            // first deactivate the connecting status of a former connecting node
            RevokeConnectRequestStatus();

            NodeVMs.Where(node => !node.Equals(sourceNode)).ForEach(node => node.ConnectRequested(isConnecting, sourceNode, connIdx, isOutBound));

            UpdateConnectingNodeData(isConnecting, sourceNode, connIdx, isOutBound);
        }

        private void OnCreateConnection(NodeViewModel targetNode, int connIdx)
        {
            if (_connNodeData.SourceNode != null)
            {
                if (_connNodeData.IsOutBound)
                {
                    _connNodeData.SourceNode.AddOutConnection(_connNodeData.ConnIdx, targetNode, connIdx);
                }
                else
                {
                    targetNode.AddOutConnection(connIdx, _connNodeData.SourceNode, _connNodeData.ConnIdx);
                }
                RevokeConnectRequestStatus();
            }
        }

        public List<NodeViewModel> Selected
        {
            get { return NodeVMs.Where(nodeVm => nodeVm.IsSelected).ToList(); }
        }

        public int SelectedCount
        {
            get { return NodeVMs.Sum(gn => gn.IsSelected ? 1 : 0); }
        }

        public void DeselectAll()
        {
            foreach (var nodeVm in NodeVMs)
            {
                nodeVm.IsSelected = false;
            }
        }
    }
}
