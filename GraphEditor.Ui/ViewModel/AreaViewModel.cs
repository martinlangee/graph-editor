#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Ui;
using GraphEditor.Interface.Utils;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class AreaViewModel : BaseNotification, IAreaViewModel
    {
        private struct ConnectingNodeData
        {
            internal NodeViewModel SourceNode;
            internal IConnectorData ConnData;
        }

        ConnectingNodeData _connNodeData;
        private INodeConfigUi _nodeConfigUi;
        private readonly IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();

        public ObservableCollection<NodeViewModel> NodeVMs { get; set; }

        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand AddNodeCommand { get; private set; }
        public RelayCommand SwitchStatesCommand { get; private set; }
        public RelayCommand ResetStatesCommand { get; private set; }
        public RelayCommand CloseNodeConfigUiCommand { get; private set; }

        public void OnBuiltUp()
        {
            LoadCommand = new RelayCommand(o => LoadExec());
            SaveCommand = new RelayCommand(o => SaveExec());
            AddNodeCommand = new RelayCommand(o => AddNodeExec((INodeTypeData) o, new Point(-1, -1)));
            SwitchStatesCommand = new RelayCommand(o => SwitchStatesExec());
            ResetStatesCommand = new RelayCommand(o => ResetStatesExec());
            CloseNodeConfigUiCommand = new RelayCommand(o => CloseNodeConfigUiExec());

            ToolBar = new ToolBarViewModel(LoadCommand, SaveCommand, SwitchStatesCommand, ResetStatesCommand);
            AreaContextMenuItems = new ObservableCollection<INodeTypeData>();
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
            var dlg = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                ClearConfig();

                var docXml = XDocument.Load(dlg.FileName);
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
            }
        }
        private void SaveExec()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                var docXml = new XDocument();

                var configXml = new XElement(_xmlClasses.Root);

                var nodesXml = new XElement(_xmlClasses.Nodes);
                NodeVMs.ForEach(node => node.SaveToXml(nodesXml));
                configXml.Add(nodesXml);

                var connXml = new XElement(_xmlClasses.Connections);
                NodeVMs.ForEach(node => node.SaveConnectionsToXml(connXml));
                configXml.Add(connXml);

                docXml.Add(configXml);

                docXml.Save(dlg.FileName);
            }
        }

        // called by IoC container
        public void ShutDown()
        {
            UiMessageHub.Dispose();
        }

        public IToolBarViewModel ToolBar { get; private set; }

        public ObservableCollection<INodeTypeData> AreaContextMenuItems { get; private set; }

        // TODO: Raster-Breite in UI editierbar machen
        public Rect GridRect { get; } = new Rect { X = 0, Y = 0, Width = UiConst.GridWidth, Height = UiConst.GridWidth };

        public NodeViewModel AddNodeExec(INodeTypeData nodeTypeData, Point location)
        {
            var newNodeVm = new NodeViewModel(nodeTypeData, () => NodeVMs.ToList(), OnOpenNodeConfigUi);
            NodeVMs.Add(newNodeVm);

            UiMessageHub.AddNode(newNodeVm, location);

            return newNodeVm;
        }

        public INodeConfigUi NodeConfigUi { get => _nodeConfigUi; private set => SetProperty<AreaViewModel, INodeConfigUi>(ref _nodeConfigUi, value, nameof(NodeConfigUi)); }

        private void OnOpenNodeConfigUi(INodeData nodeData)
        {
            NodeConfigUi = nodeData.CreateConfigUi();
            UiMessageHub.LocationUpdateMuted = true;
            ToolBar.IsEnabled = false;
        }

        private void CloseNodeConfigUiExec()
        {
            NodeConfigUi = null;
            UiMessageHub.LocationUpdateMuted = false;
            ToolBar.IsEnabled = true;
        }

        public void OnRemoveNode(NodeViewModel nodeVm)
        {
            NodeVMs.Remove(nodeVm);
        }

        public void RevokeConnectRequestStatus()
        {
            Application.Current.MainWindow.Cursor = Cursors.Arrow;

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

        public IList<INodeViewModel> Selected => NodeVMs.Where(nodeVm => nodeVm.IsSelected).Cast<INodeViewModel>().ToList(); 

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
