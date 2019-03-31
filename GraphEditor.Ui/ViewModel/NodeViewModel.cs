using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Utils;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace GraphEditor.Ui.ViewModel
{
    public class NodeViewModel : BaseNotification
    {
        private Func<List<NodeViewModel>> _onGetAllNodeVMs;
        private bool _isSelected = false;
        private Point _location;

        public NodeViewModel(Func<List<NodeViewModel>> onGetAllNodeVMs, INodeTypeData nodeTypeData)
        {
            _onGetAllNodeVMs = onGetAllNodeVMs;
            NodeData = nodeTypeData.CreateNode();

            InConnectors = new ObservableCollection<ConnectorStateViewModel>();
            OutConnectors = new ObservableCollection<ConnectorStateViewModel>();

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            EditConfigCommand = new RelayCommand(o => EditConfigExec());
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            Type = NodeData.TypeData.Name;
            Name = NodeData.Name;

            for (int i = 0; i < NodeData.InConnectors.Count; i++)
                InConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: false));

            for (int i = 0; i < NodeData.OutConnectors.Count; i++)
                OutConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: true));
        }

        private void EditConfigExec()
        {
            
        }

        internal void RemoveConnection(ConnectionViewModel connVm)
        {
            OutConnections.Remove(connVm);

            OutConnectors[connVm.SourceConnector].IsConnected = false;
            connVm.TargetNode.InConnectors[connVm.TargetConnector].IsConnected = false;

            UiMessageHub.RemoveConnection(connVm);
        }

        internal void ConnectRequested(bool isConnecting, NodeViewModel otherNode, int otherConnIdx, bool isOutBound)
        {
            if (isOutBound)
            {
                if (isConnecting)
                    InConnectors.For((conn, idx) => conn.IsConnectRequested = otherNode.NodeData.TypeData.CanConnectToIn(NodeData.TypeData, otherConnIdx, idx));
                else
                    InConnectors.ForEach(conn => conn.IsConnectRequested = false);
            }
            else
            {
                if (isConnecting)
                    OutConnectors.For((conn, idx) => conn.IsConnectRequested = otherNode.NodeData.TypeData.CanConnectToOut(NodeData.TypeData, otherConnIdx, idx));
                else
                    OutConnectors.ForEach(conn => conn.IsConnectRequested = false);
            }
        }

        public RelayCommand EditConfigCommand { get; }
        public RelayCommand RemoveNodeCommand { get; }

        public INodeData NodeData { get; }

        public string Type { get; set; }
        public string Name { get; set; }

        public ObservableCollection<ConnectorStateViewModel> InConnectors { get; }

        public ObservableCollection<ConnectionViewModel> OutConnections { get; }

        public void AddOutConnection(int sourceConn, NodeViewModel targetConnVm, int targetConn)
        {
            var connVm = new ConnectionViewModel(this, targetConnVm, sourceConn, targetConn);
            OutConnections.Add(connVm);
            OutConnectors[connVm.SourceConnector].IsConnected = true;
            targetConnVm.InConnectors[connVm.TargetConnector].IsConnected = true;
            UiMessageHub.AddConnection(connVm);
        }

        public ObservableCollection<ConnectorStateViewModel> OutConnectors { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty<NodeViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }
        }

        public Point Location
        {
            get => _location;
            set
            {
                SetProperty<NodeViewModel, Point>(ref _location, value, nameof(Location),
                    (node, pt) => UiMessageHub.NodeLocationChanged(this, pt));
            }
        }


        public void RemoveExec()
        {
            new List<ConnectionViewModel>(OutConnections).ForEach(conn => conn.Remove());
            InConnections.ForEach(ic => ic.Remove());
            UiMessageHub.RemoveNode(this);
        }

        private List<ConnectionViewModel> InConnections
        {
            get
            {
                var nodeVMs = _onGetAllNodeVMs();
                var result = new List<ConnectionViewModel>();

                nodeVMs.Where(nv => !nv.Equals(this)).ToList().
                    ForEach(nv => nv.OutConnections.Where(conn => conn.TargetNode.Equals(this)).ToList().
                    ForEach(conn => result.Add(conn)));

                return result;
            }
        }
    }
}