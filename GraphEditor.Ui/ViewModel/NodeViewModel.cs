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
        private bool _isSelected = false;
        private Point _location;

        public NodeViewModel(AreaViewModel area)
        {
            Area = area;

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
            MessageHub.Inst.RemoveConnection(connVm);
        }

        // TODO: per inversion of control hier raus nehmen?
        public AreaViewModel Area { get; }

        internal void ConnectRequested(bool value, NodeViewModel sourceNode, int connectorIdx, bool isOutBound)
        {
            if (isOutBound)
            {
                if (value)
                    InConnectors.Where(conn => conn.Index % 2 == connectorIdx % 2).ToList().ForEach(conn => conn.IsConnectRequested = true);
                else
                    InConnectors.ToList().ForEach(conn => conn.IsConnectRequested = false);
            }
            else
            {
                if (value)
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
            MessageHub.Inst.AddConnection(connVm);
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
                    (node, pt) => MessageHub.Inst.NodeLocationChanged(this, pt));
            }
        }


        public void RemoveExec()
        {
            new List<ConnectionViewModel>(OutConnections).ForEach(conn => conn.Remove());
            InConnections.ForEach(ic => ic.Remove());
            MessageHub.Inst.RemoveNode(this);
        }

        private List<ConnectionViewModel> InConnections
        {
            get
            {
                var nodeVMs = new List<NodeViewModel>(Area.NodeVMs);
                var result = new List<ConnectionViewModel>();

                nodeVMs.Where(nv => !nv.Equals(this)).ToList().
                    ForEach(nv => nv.OutConnections.Where(conn => conn.TargetNode.Equals(this)).ToList().
                    ForEach(conn => result.Add(conn)));

                return result;
            }
        }
    }
}