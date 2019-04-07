using System;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Ui;

namespace GraphEditor.Nodes.ViewModel
{
    public class LogicalAND : NodeDataBase
    {
        public LogicalAND(INodeTypeData nodeTypeData, Func<IConnectorData, bool> checkIsConnected) 
            : base(nodeTypeData, checkIsConnected)
        {
            InConnectors.Add(new ConnectorData("IN 1", 0, false, true, checkIsConnected));
            InConnectors.Add(new ConnectorData("IN 2", 1, false, false, checkIsConnected));
            InConnectors.Add(new ConnectorData("IN 3", 2, false, true, checkIsConnected));

            OutConnectors.Add(new ConnectorData("OUT (AND)", 0, true, true, checkIsConnected));
        }

        protected override Type ConfigControlType => typeof(LogicalAND_ctrl);
    }
}