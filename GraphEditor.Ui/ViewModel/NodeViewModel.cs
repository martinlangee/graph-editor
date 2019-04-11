using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Utils;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class NodeViewModel : BaseNotification
    {
        private readonly Func<List<NodeViewModel>> _onGetAllNodeVMs;
        private readonly Action<INodeConfigUi> _onOpenConfigUi;
        private bool _isSelected = false;
        private Point _location;

        public NodeViewModel(IBaseNodeTypeData nodeTypeData, Func<List<NodeViewModel>> onGetAllNodeVMs, Action<INodeConfigUi> onOpenConfigUi)
        {
            _onGetAllNodeVMs = onGetAllNodeVMs;
            _onOpenConfigUi = onOpenConfigUi;
            Data = nodeTypeData.CreateNode(ConnectorOnActiveChanged, ConnectorCanBeDeactivated);

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            EditConfigCommand = new RelayCommand(o => EditConfigExec());
            ShiftZOrderCommand = new RelayCommand(b => ShiftZOrderExec((bool) b));
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            InConnectorStates = new ObservableCollection<ConnectorStateViewModel>();
            OutConnectorStates = new ObservableCollection<ConnectorStateViewModel>();

            LoadInConnectorStates();
            LoadOutConnectorStates();
        }

        private void ConnectorOnActiveChanged(IBaseConnectorData connectorData)
        {
            var connectorStates = connectorData.IsOutBound ? OutConnectorStates : InConnectorStates;
            var connections = connectorData.IsOutBound ? OutConnections.ToList() : InConnections;

            connections.ForEach(conn =>
            {
                var connIdx = connectorData.IsOutBound ? conn.SourceConnector : conn.TargetConnector;
                if (connIdx > connectorData.Index)
                {
                    var connectedPointIdx = connectorData.IsOutBound ? 0 : conn.LastPointIndex;
                    var nextPointIdx = connectorData.IsOutBound ? 1 : conn.LastPointIndex - 1;

                    // if the connector has one or more bend points and it is on the same height as the connector => also shift it up or down
                    if (conn.LastPointIndex > 1 &&
                        Math.Round(conn.Points[nextPointIdx].Y).Equals(Math.Round(conn.Points[connectedPointIdx].Y)))
                    {
                        conn.MovePoint(nextPointIdx, down: connectorData.IsActive);
                    }

                    conn.MovePoint(connectedPointIdx, down: connectorData.IsActive);
                }
            });

            connectorStates[connectorData.Index].IsActive = connectorData.IsActive;
        }

        private bool ConnectorCanBeDeactivated(IBaseConnectorData connData)
        {
            var connectorStates = connData.IsOutBound ? OutConnectorStates : InConnectorStates;

            return connectorStates?.Count(cs => cs.IsActive) > 1 &&
                   connectorStates?[connData.Index].IsConnected == false;
        }

        private void LoadConnectorStates(IList<IBaseConnectorData> connectors, ObservableCollection<ConnectorStateViewModel> connStateVMs, bool isOutBound)
        {
            connectors.For((ic, i) => connStateVMs.Add(new ConnectorStateViewModel(this, ic.Name, i, isOutBound)));
        }

        private void LoadInConnectorStates()
        {
            LoadConnectorStates(Data.Ins, InConnectorStates, isOutBound: false);
        }

        private void LoadOutConnectorStates()
        {
            LoadConnectorStates(Data.Outs, OutConnectorStates, isOutBound: true);
        }

        private void EditConfigExec()
        {
            _onOpenConfigUi(Data.CreateConfigUi());
        }

        private void ShiftZOrderExec(bool up)
        {
            UiMessageHub.ShiftZOrder(this, up);
        }

        internal void RemoveConnection(ConnectionViewModel connVm)
        {
            OutConnections.Remove(connVm);

            OutConnectorStates[connVm.SourceConnector].IsConnected = false;
            connVm.TargetNode.InConnectorStates[connVm.TargetConnector].IsConnected = false;

            UiMessageHub.RemoveConnection(connVm);
        }

        internal void ConnectRequested(bool isConnecting, NodeViewModel otherNode, int otherConnIdx, bool isOutBound)
        {
            if (isOutBound)
            {
                if (isConnecting)
                    InConnectorStates.For((conn, idx) => conn.IsConnectRequested = otherNode.Data.TypeData.CanConnectToIn(Data.TypeData, otherConnIdx, idx));
                else
                    InConnectorStates.ForEach(conn => conn.IsConnectRequested = false);
            }
            else
            {
                if (isConnecting)
                    OutConnectorStates.For((conn, idx) => conn.IsConnectRequested = otherNode.Data.TypeData.CanConnectToOut(Data.TypeData, otherConnIdx, idx));
                else
                    OutConnectorStates.ForEach(conn => conn.IsConnectRequested = false);
            }
        }

        public RelayCommand EditConfigCommand { get; }

        public RelayCommand ShiftZOrderCommand { get; }

        public RelayCommand RemoveNodeCommand { get; }

        public INodeData Data { get; }

        public void LoadFromXml(XElement nodeXml)
        {
            Location = nodeXml.Attribute("Location").Value.ToPoint();
            Data.LoadFromXml(nodeXml);
        }

        public void SaveToXml(XElement parentXml)
        {
            var nodeVmXml = new XElement("Node");
            nodeVmXml.SetAttributeValue("Location", Location);
            Data.SaveToXml(nodeVmXml);
            parentXml.Add(nodeVmXml);
        }

        public void SaveConnectionsToXml(XElement parentXml)
        {
            OutConnections.ForEach(conn => conn.SaveToXml(parentXml));
        }

        public ObservableCollection<ConnectorStateViewModel> InConnectorStates { get; }

        public ObservableCollection<ConnectorStateViewModel> OutConnectorStates { get; }

        public bool IsSelected { get => _isSelected; set => SetProperty<NodeViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }

        public Point Location { get => _location; set => SetProperty<NodeViewModel, Point>(ref _location, value, nameof(Location), (node, pt) => UiMessageHub.NodeLocationChanged(this, pt)); }

        public void RemoveExec()
        {
            new List<ConnectionViewModel>(OutConnections).ForEach(conn => conn.Remove());
            InConnections.ForEach(ic => ic.Remove());
            UiMessageHub.RemoveNode(this);
        }

        public ObservableCollection<ConnectionViewModel> OutConnections { get; }

        public ConnectionViewModel AddOutConnection(int sourceConn, NodeViewModel targetConnVm, int targetConn)
        {
            var connVm = new ConnectionViewModel(this, targetConnVm, sourceConn, targetConn);
            OutConnections.Add(connVm);
            OutConnectorStates[connVm.SourceConnector].IsConnected = true;
            targetConnVm.InConnectorStates[connVm.TargetConnector].IsConnected = true;
            UiMessageHub.AddConnection(connVm);

            return connVm;
        }

        private List<ConnectionViewModel> InConnections
        {
            get
            {
                var nodeVMs = _onGetAllNodeVMs();
                var result = new List<ConnectionViewModel>();

                nodeVMs.Where(nv => !nv.Equals(this)).
                    ForEach(nv => nv.OutConnections.Where(conn => conn.TargetNode.Equals(this)).
                    ForEach(conn => result.Add(conn)));

                return result;
            }
        }
    }
}