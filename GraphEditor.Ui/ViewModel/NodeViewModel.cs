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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class NodeViewModel : BaseNotification, INodeViewModel
    {
        private readonly IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();
        private readonly Func<List<NodeViewModel>> _onGetAllNodeVMs;
        private readonly Action<INodeData> _onOpenConfigUi;
        private bool _isSelected = false;
        private Point _location;

        public NodeViewModel()
        { }

        public NodeViewModel(INodeTypeData nodeTypeData, Func<List<NodeViewModel>> onGetAllNodeVMs, Action<INodeData> onOpenConfigUi)
        {
            _onGetAllNodeVMs = onGetAllNodeVMs;
            _onOpenConfigUi = onOpenConfigUi;
            Data = nodeTypeData.CreateNode(ConnectorOnActiveChanged, ConnectorCanBeDeactivated);

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            EditConfigCommand = new RelayCommand(o => EditConfigExec());
            ShiftZOrderCommand = new RelayCommand(b => ShiftZOrderExec((bool) b));
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            InConnectorStates = new ObservableCollection<ConnectorViewModel>();
            OutConnectorStates = new ObservableCollection<ConnectorViewModel>();

            LoadInConnectorStates();
            LoadOutConnectorStates();
        }

        private void ConnectorOnActiveChanged(IConnectorData connectorData)
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

        private bool ConnectorCanBeDeactivated(IConnectorData connData)
        {
            var connectorStates = connData.IsOutBound ? OutConnectorStates : InConnectorStates;

            return connectorStates?.Count(cs => cs.IsActive) > 1 &&
                   connectorStates?[connData.Index].IsConnected == false;
        }

        private void LoadConnectorStates(IList<IConnectorData> connectors, ObservableCollection<ConnectorViewModel> connStateVMs, bool isOutBound)
        {
            connectors.For((ic, i) => connStateVMs.Add(ConnectorViewModel.Create(this, ic.Name, i, isOutBound)));
        }

        private void LoadInConnectorStates()
        {
            LoadConnectorStates(Data.Ins, InConnectorStates, isOutBound: false);
        }

        private void LoadOutConnectorStates()
        {
            LoadConnectorStates(Data.Outs, OutConnectorStates, isOutBound: true);
        }

        public void EditConfigExec()
        {
            _onOpenConfigUi(Data);
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

        internal void ConnectRequested(bool isConnecting, IConnectorData connData)
        {
            var connectorStates = connData.IsOutBound ? InConnectorStates : OutConnectorStates;

            if (isConnecting)
                connectorStates.For((conn, idx) => conn.IsConnectRequested = Data.CanConnectTo(idx, connData));
            else
                connectorStates.ForEach(conn => conn.IsConnectRequested = false);
        }

        public RelayCommand EditConfigCommand { get; }

        public RelayCommand ShiftZOrderCommand { get; }

        public RelayCommand RemoveNodeCommand { get; }

        public INodeData Data { get; }

        public void LoadFromXml(XElement nodeXml)
        {
            Location = nodeXml.Attribute(_xmlClasses.Location).Value.ToPoint();
            Data.LoadFromXml(nodeXml);
        }

        public void SaveToXml(XElement parentXml)
        {
            var nodeVmXml = new XElement(_xmlClasses.Node);
            nodeVmXml.SetAttributeValue(_xmlClasses.Location, Location);
            Data.SaveToXml(nodeVmXml);
            parentXml.Add(nodeVmXml);
        }

        public void SaveConnectionsToXml(XElement parentXml)
        {
            OutConnections.ForEach(conn => conn.SaveToXml(parentXml));
        }

        public ObservableCollection<ConnectorViewModel> InConnectorStates { get; }

        public ObservableCollection<ConnectorViewModel> OutConnectorStates { get; }

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