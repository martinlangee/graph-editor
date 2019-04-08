using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Utils;
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
        private Func<List<NodeViewModel>> _onGetAllNodeVMs;
        private Action<INodeConfigUi> _onOpenConfigUi;
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

            InConnectors = new ObservableCollection<ConnectorStateViewModel>();
            OutConnectors = new ObservableCollection<ConnectorStateViewModel>();

            ReloadInConnectors();
            ReloadOutConnectors();
        }

        private void ConnectorOnActiveChanged(IConnectorData connectorData, bool isActive)
        {
            if (connectorData.IsOutBound)
                ReloadOutConnectors();
            else
                ReloadInConnectors();
        }

        private bool ConnectorCanBeDeactivated(IConnectorData connData)
        {
            var connectors = connData.IsOutBound ? OutConnectors : InConnectors;

            if (connectors?.Count <= 1)
                return false;

            var connState = connectors?.FirstOrDefault(ic => ic.Index == connData.Index && ic.IsOutBound == connData.IsOutBound);
            return connState?.IsConnected == false;
        }

        private void ReloadConnectors(IList<IConnectorData> connectors, ObservableCollection<ConnectorStateViewModel> connStateVMs, bool isOutBound)
        {
            connStateVMs.Clear();

            connectors.
                For((ic, i) =>
                {
                    if (ic.IsActive)
                        connStateVMs.Add(new ConnectorStateViewModel(this, ic.Name, i, isOutBound));
                });
        }

        private void ReloadInConnectors()
        {
            ReloadConnectors(Data.Ins, InConnectors, isOutBound: false);
        }

        private void ReloadOutConnectors()
        {
            ReloadConnectors(Data.Outs, OutConnectors, isOutBound: true);
        }

        private void EditConfigExec()
        {
            _onOpenConfigUi(Data.CreateConfigUi());
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
                    InConnectors.For((conn, idx) => conn.IsConnectRequested = otherNode.Data.TypeData.CanConnectToIn(Data.TypeData, otherConnIdx, idx));
                else
                    InConnectors.ForEach(conn => conn.IsConnectRequested = false);
            }
            else
            {
                if (isConnecting)
                    OutConnectors.For((conn, idx) => conn.IsConnectRequested = otherNode.Data.TypeData.CanConnectToOut(Data.TypeData, otherConnIdx, idx));
                else
                    OutConnectors.ForEach(conn => conn.IsConnectRequested = false);
            }
        }

        public RelayCommand EditConfigCommand { get; }
        public RelayCommand RemoveNodeCommand { get; }

        public INodeData Data { get; }

        public void LoadNodeFromXml(XElement nodeXml)
        {
            Data.LoadFromXml(nodeXml);
        }

        public void SaveNodeToXml(XElement parentXml)
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

        public ObservableCollection<ConnectorStateViewModel> InConnectors { get; }

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

        public ObservableCollection<ConnectionViewModel> OutConnections { get; }

        public void AddOutConnection(int sourceConn, NodeViewModel targetConnVm, int targetConn)
        {
            var connVm = new ConnectionViewModel(this, targetConnVm, sourceConn, targetConn);
            OutConnections.Add(connVm);
            OutConnectors[connVm.SourceConnector].IsConnected = true;
            targetConnVm.InConnectors[connVm.TargetConnector].IsConnected = true;
            UiMessageHub.AddConnection(connVm);
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