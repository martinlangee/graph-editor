using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Utils;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace GraphEditor.Ui.ViewModel
{
    public class NodeViewModel : BaseNotification
    {
        private Func<List<NodeViewModel>> _onGetAllNodeVMs;
        private Action<INodeConfigUi> _onOpenConfigUi;
        private bool _isSelected = false;
        private Point _location;

        public NodeViewModel(INodeTypeData nodeTypeData, Func<List<NodeViewModel>> onGetAllNodeVMs, Action<INodeConfigUi> onOpenConfigUi)
        {
            _onGetAllNodeVMs = onGetAllNodeVMs;
            _onOpenConfigUi = onOpenConfigUi;
            NodeData = nodeTypeData.CreateNode();

            NodeData.OnInConnectorsChanged = e => ReloadInConnectors(e);
            NodeData.OnOutConnectorsChanged = e => ReloadOutConnectors(e);

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            EditConfigCommand = new RelayCommand(o => EditConfigExec());
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            InConnectors = new ObservableCollection<ConnectorStateViewModel>();
            OutConnectors = new ObservableCollection<ConnectorStateViewModel>();

            ReloadInConnectors();
            ReloadOutConnectors();
        }

        private static bool CanChangeConnectors(ObservableCollection<ConnectorStateViewModel> connectors, NotifyCollectionChangedEventArgs e)
        {
            if (e == null) return true;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (connectors.Any(c => c.IsConnected)) return false;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var conn in connectors.Reverse())
                {
                    if (conn.Index > (e.NewItems.Count - 1) && conn.IsConnected)
                        return false;
                }
            }

            return true;
        }

        private bool ReloadInConnectors(NotifyCollectionChangedEventArgs e = null)
        {
            if (!CanChangeConnectors(InConnectors, e)) return false;

            for (int i = 0; i < NodeData.InConnectors.Count; i++)
                InConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: false));

            return true;
        }

        private bool ReloadOutConnectors(NotifyCollectionChangedEventArgs e = null)
        {
            if (!CanChangeConnectors(OutConnectors, e)) return false;

            for (int i = 0; i < NodeData.OutConnectors.Count; i++)
                OutConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: true));

            return true;
        }

        private void EditConfigExec()
        {
            _onOpenConfigUi(NodeData.CreateConfigUi());
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