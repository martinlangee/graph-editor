using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Container;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Utils;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        ConnectingNodeData _connectingNode;
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
            NodeVMs.InverseFor((node, i) => UiMessageHub.RemoveNode(node));
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
            nodesXml.Elements().ForEach(el =>
            {
                var nodeVm = AddNodeExec(ServiceContainer.Get<INodeTypeRepository>().Find(el.Attribute("Type").Value));
                var pts = el.Attribute("Location").Value.Split(';');
                nodeVm.Location = Point.Parse(string.Join(",", pts));

                nodeVm.LoadNodeFromXml(el);
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
            if (_connectingNode.SourceNode != null)
            {
                if (_connectingNode.IsOutBound)
                    _connectingNode.SourceNode.OutConnectors[_connectingNode.ConnIdx].IsConnecting = false;
                else
                    _connectingNode.SourceNode.InConnectors[_connectingNode.ConnIdx].IsConnecting = false;
            }
        }

        private void UpdateConnectingNodeData(bool isConnecting, NodeViewModel sourceNode, int connIdx, bool isOutBound)
        {
            if (isConnecting)
            {
                _connectingNode.SourceNode = sourceNode;
                _connectingNode.ConnIdx = connIdx;
                _connectingNode.IsOutBound = isOutBound;
            }
            else
                _connectingNode.SourceNode = null;
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
            if (_connectingNode.SourceNode != null)
            {
                if (_connectingNode.IsOutBound)
                {
                    _connectingNode.SourceNode.AddOutConnection(_connectingNode.ConnIdx, targetNode, connIdx);
                }
                else
                {
                    targetNode.AddOutConnection(connIdx, _connectingNode.SourceNode, _connectingNode.ConnIdx);
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
