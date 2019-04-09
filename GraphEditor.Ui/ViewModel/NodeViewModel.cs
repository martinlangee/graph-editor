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

        public NodeViewModel(INodeTypeData nodeTypeData, Func<List<NodeViewModel>> onGetAllNodeVMs, Action<INodeConfigUi> onOpenConfigUi)
        {
            _onGetAllNodeVMs = onGetAllNodeVMs;
            _onOpenConfigUi = onOpenConfigUi;
            Data = nodeTypeData.CreateNode(ConnectorOnActiveChanged, ConnectorCanBeDeactivated);

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            EditConfigCommand = new RelayCommand(o => EditConfigExec());
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            InConnectorStates = new ObservableCollection<ConnectorStateViewModel>();
            OutConnectorStates = new ObservableCollection<ConnectorStateViewModel>();

            LoadInConnectorStates();
            LoadOutConnectorStates();
        }

        private void ConnectorOnActiveChanged(IConnectorData connectorData)
        {
            var connectorStates = connectorData.IsOutBound ? OutConnectorStates : InConnectorStates;
            var connections = connectorData.IsOutBound ? OutConnections.ToList() : InConnections;

            if (connectorData.IsActive)
            {
                connectorStates.Insert(connectorData.Index, new ConnectorStateViewModel(this, connectorData.Name, connectorData.Index, connectorData.IsOutBound));
                // todo connections.
            }
            else
            {
                connectorStates.RemoveAt(connectorData.Index);
            }
        }

        private bool ConnectorCanBeDeactivated(IConnectorData connData)
        {
            var connectors = connData.IsOutBound ? OutConnectorStates : InConnectorStates;

            if (connectors?.Count <= 1)
                return false;

            var connState = connectors?.FirstOrDefault(ic => ic.Index == connData.Index && ic.IsOutBound == connData.IsOutBound);
            return connState?.IsConnected == false;
        }

        private void ReloadConnectorStates(IList<IConnectorData> connectors, ObservableCollection<ConnectorStateViewModel> connStateVMs, bool isOutBound)
        {
            connStateVMs.Clear();

            connectors.For((ic, i) =>
                {
                    if (ic.IsActive)
                        connStateVMs.Add(new ConnectorStateViewModel(this, ic.Name, i, isOutBound));
                });
        }

        private void LoadInConnectorStates()
        {
            ReloadConnectorStates(Data.Ins, InConnectorStates, isOutBound: false);
        }

        private void LoadOutConnectorStates()
        {
            ReloadConnectorStates(Data.Outs, OutConnectorStates, isOutBound: true);
        }

        private void EditConfigExec()
        {
            _onOpenConfigUi(Data.CreateConfigUi());
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

                nodeVMs.Where(nv => !nv.Equals(this)).ToList().
                    ForEach(nv => nv.OutConnections.Where(conn => conn.TargetNode.Equals(this)).ToList().
                    ForEach(conn => result.Add(conn)));

                return result;
            }
        }
    }
}