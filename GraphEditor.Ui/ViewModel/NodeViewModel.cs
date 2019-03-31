using GraphEditor.Commands;
using GraphEditor.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class NodeViewModel : BaseNotification
    {
        private Func<List<NodeViewModel>> _onGetAllNodeVMs;
        private bool _isSelected = false;
        private Point _location;

        public NodeViewModel(Func<List<NodeViewModel>> onGetAllNodeVMs)
        {
            _onGetAllNodeVMs = onGetAllNodeVMs;

            InConnectors = new ObservableCollection<ConnectorStateViewModel>();
            OutConnectors = new ObservableCollection<ConnectorStateViewModel>();

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            // todo: provisorisch
            for (int i = 0; i < 5; i++)
                InConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: false));

            for (int i = 0; i < 5; i++)
                OutConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: true));
        }

        internal void RemoveConnection(ConnectionViewModel connVm)
        {
            OutConnections.Remove(connVm);

            OutConnectors[connVm.SourceConnector].IsConnected = false;
            connVm.TargetNode.InConnectors[connVm.TargetConnector].IsConnected = false;

            UiMessageHub.RemoveConnection(connVm);
        }

        internal void ConnectRequested(bool isConnecting, NodeViewModel sourceNode, int connectorIdx, bool isOutBound)
        {
            if (isOutBound)
            {
                if (isConnecting)
                    // TODO: provisorisch; welche Connectors als Target erlaubt sind muss in konkreten Node-Klassen bestimmt werden
                    InConnectors.Where(conn => conn.Index % 2 == connectorIdx % 2).ToList().ForEach(conn => conn.IsConnectRequested = true);
                else
                    InConnectors.ToList().ForEach(conn => conn.IsConnectRequested = false);
            }
            else
            {
                if (isConnecting)
                    // TODO: provisorisch; welche Connectors als Target erlaubt sind muss in konkreten Node-Klassen bestimmt werden
                    OutConnectors.Where(conn => conn.Index % 2 == connectorIdx % 2).ToList().ForEach(conn => conn.IsConnectRequested = true);
                else
                    OutConnectors.ToList().ForEach(conn => conn.IsConnectRequested = false);
            }
        }

        public RelayCommand ConnectToCommand { get; }
        public RelayCommand RemoveNodeCommand { get; }

        public string Type { get; set; } = "Filter";
        public string Name { get; set; } = "Neu";

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