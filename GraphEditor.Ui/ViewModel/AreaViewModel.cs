using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Utils;
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
using System.Windows.Media;
using System.Xml.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class AreaViewModel : BaseNotification
    {
        private struct ConnectingNodeData
        {
            internal NodeViewModel SourceNode;
            internal IConnectorData ConnData;
        }

        ConnectingNodeData _connNodeData;
        private UserControl _nodeConfigUi;
        private readonly IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();

        public ObservableCollection<NodeViewModel> NodeVMs { get; set; }

        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand AddNodeCommand { get; private set; }
        public RelayCommand SwitchStatesCommand { get; private set; }
        public RelayCommand ResetStatesCommand { get; private set; }

        public void OnBuiltUp()
        {
            LoadCommand = new RelayCommand(o => LoadExec());
            SaveCommand = new RelayCommand(o => SaveExec());
            AddNodeCommand = new RelayCommand(o => AddNodeExec((IBaseNodeTypeData) o, new Point(-1, -1)));
            SwitchStatesCommand = new RelayCommand(o => SwitchStatesExec());
            ResetStatesCommand = new RelayCommand(o => ResetStatesExec());

            ToolBar = new ToolBarViewModel(LoadCommand, SaveCommand, SwitchStatesCommand, ResetStatesCommand);
            AreaContextMenuItems = new ObservableCollection<IBaseNodeTypeData>();
            NodeVMs = new ObservableCollection<NodeViewModel>();

            ServiceContainer.Get<INodeTypeRepository>().NodeTypes.ForEach(
                nt => AreaContextMenuItems.Add(nt)
            );

            UiMessageHub.OnRemoveNode += OnRemoveNode;
            UiMessageHub.OnConnectRequested += OnConnectRequested;
            UiMessageHub.OnCreateConnection = OnCreateConnection;
        }

        public void OnTearDown()
        {
            ClearConfig();
        }

        private void ClearConfig()
        {
            NodeVMs.Reverse().ForEach(node => node.RemoveExec());
        }

        private void LoadExec()
        {
            //var dlg = new OpenFileDialog();
            //if (dlg.ShowDialog() == true)
            //{
            ClearConfig();

            var docXml = XDocument.Load("c:\\gn.xml");
            var configXml = docXml.Element(_xmlClasses.Root);
            var nodesXml = configXml.Element(_xmlClasses.Nodes);
            nodesXml.Elements().ForEach(node =>
            {
                var nodeVm = AddNodeExec(ServiceContainer.Get<INodeTypeRepository>().Find(node.Attribute(_xmlClasses.Type).Value), new Point(-1, -1));
                nodeVm.LoadFromXml(node);
            });

            // this is a workaround to ensure that the nodes are loaded and all bound item collections initialized before the connections are loaded
            Task.Run(() =>
            {
                Thread.Sleep(200);  // important!

                var connectionsXml = configXml.Element(_xmlClasses.Connections);
                connectionsXml.Elements().ForEach(conn =>
                {
                    var sourceId = conn.Attribute(_xmlClasses.Source).Value;
                    var sourceConn = int.Parse(conn.Attribute(_xmlClasses.SourceConn).Value);
                    var targetId = conn.Attribute(_xmlClasses.Target).Value;
                    var targetConn = int.Parse(conn.Attribute(_xmlClasses.TargetConn).Value);

                    _connNodeData.SourceNode = NodeVMs.First(node => node.Data.Id == sourceId);
                    _connNodeData.ConnData = _connNodeData.SourceNode.Data.Outs[sourceConn];

                    var targetNode = NodeVMs.First(node => node.Data.Id == targetId);

                    CurrentDispatcher.Invoke(() =>
                    {
                        OnCreateConnection(targetNode, targetConn).LoadFromToXml(conn);
                    });
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

            var configXml = new XElement(_xmlClasses.Root);

            var nodesXml = new XElement(_xmlClasses.Nodes);
            NodeVMs.ForEach(node => node.SaveToXml(nodesXml));
            configXml.Add(nodesXml);

            var connXml = new XElement(_xmlClasses.Connections);
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

        public ObservableCollection<IBaseNodeTypeData> AreaContextMenuItems { get; private set; }

        // TODO: Raster-Breite in UI editierbar machen
        public Rect GridRect { get; } = new Rect { X = 0, Y = 0, Width = UiConst.GridWidth, Height = UiConst.GridWidth };

        public NodeViewModel AddNodeExec(IBaseNodeTypeData nodeTypeData, Point location)
        {
            var newNodeVm = new NodeViewModel(nodeTypeData, () => NodeVMs.ToList(), OnOpenConfigUi);
            NodeVMs.Add(newNodeVm);

            UiMessageHub.AddNode(newNodeVm, location);

            return newNodeVm;
        }

        public UserControl NodeConfigUi { get => _nodeConfigUi; private set => SetProperty<AreaViewModel, UserControl>(ref _nodeConfigUi, value, nameof(NodeConfigUi)); }

        private void OnOpenConfigUi(INodeConfigUi configUi)
        {
            configUi.OnClose += ui => 
            {
                NodeConfigUi = null;
                UiMessageHub.LocationUpdateMuted = false;
                ToolBar.IsEnabled = true;
            };
            NodeConfigUi = configUi as UserControl;
            UiMessageHub.LocationUpdateMuted = true;
            ToolBar.IsEnabled = false;
        }

        public void OnRemoveNode(NodeViewModel nodeVm)
        {
            NodeVMs.Remove(nodeVm);
        }

        public void RevokeConnectRequestStatus()
        {
            if (_connNodeData.SourceNode != null)
            {
                if (_connNodeData.ConnData.IsOutBound)
                    _connNodeData.SourceNode.OutConnectorStates[_connNodeData.ConnData.Index].IsConnecting = false;
                else
                    _connNodeData.SourceNode.InConnectorStates[_connNodeData.ConnData.Index].IsConnecting = false;
            }
        }

        private void UpdateConnectingNodeData(bool isConnecting, NodeViewModel sourceNode, IConnectorData connData)
        {
            if (isConnecting)
            {
                _connNodeData.SourceNode = sourceNode;
                _connNodeData.ConnData = connData;
            }
            else
                _connNodeData.SourceNode = null;
        }

        private void OnConnectRequested(bool isConnecting, NodeViewModel sourceNode, IConnectorData connData)
        {
            // first deactivate the connecting status of a former connecting node
            RevokeConnectRequestStatus();

            NodeVMs.Where(node => !node.Equals(sourceNode)).ForEach(node => node.ConnectRequested(isConnecting, connData));

            UpdateConnectingNodeData(isConnecting, sourceNode, connData);
        }

        private ConnectionViewModel OnCreateConnection(NodeViewModel targetNode, int connIdx)
        {
            ConnectionViewModel connVm = null;

            if (_connNodeData.SourceNode != null)
            {
                if (_connNodeData.ConnData.IsOutBound)
                {
                     connVm = _connNodeData.SourceNode.AddOutConnection(_connNodeData.ConnData.Index, targetNode, connIdx);
                }
                else
                {
                    connVm = targetNode.AddOutConnection(connIdx, _connNodeData.SourceNode, _connNodeData.ConnData.Index);
                }
                RevokeConnectRequestStatus();
            }

            return connVm;
        }

        public List<NodeViewModel> Selected => NodeVMs.Where(nodeVm => nodeVm.IsSelected).ToList(); 

        public int SelectedCount => NodeVMs.Sum(gn => gn.IsSelected ? 1 : 0); 

        public void DeselectAll()
        {
            foreach (var nodeVm in NodeVMs)
            {
                nodeVm.IsSelected = false;
            }
        }

        private void SwitchStatesExec()
        {
            // example for the use of State as connections visualization color 
            NodeVMs.For((node, i) => node.OutConnections.For((conn, j) =>
            {
                switch (j % 4)
                {
                    case 0:
                        conn.State = Brushes.Red;
                        break;
                    case 1:
                        conn.State = Brushes.Green;
                        break;
                    case 2:
                        conn.State = Brushes.Orange;
                        break;
                    case 3:
                        conn.State = Brushes.DarkCyan;
                        break;
                }
            }));
        }

        private void ResetStatesExec()
        {
            NodeVMs.ForEach(node => node.OutConnections.ForEach(conn => conn.State = Brushes.Gray));
        }
    }
}
