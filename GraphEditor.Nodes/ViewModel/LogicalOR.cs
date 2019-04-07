using System;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Ui;

namespace GraphEditor.Nodes.ViewModel
{
    public class LogicalOR : NodeDataBase
    {
        public LogicalOR(INodeTypeData nodeTypeData, Func<IConnectorData, bool> checkIsConnected) 
            : base(nodeTypeData, checkIsConnected)
        {
            InConnectors.Add(new ConnectorData("IN 1", 0, false, true, checkIsConnected));
            InConnectors.Add(new ConnectorData("IN 2", 1, false, true, checkIsConnected));
            InConnectors.Add(new ConnectorData("IN 3", 2, false, true, checkIsConnected));
            InConnectors.Add(new ConnectorData("IN 4", 3, false, true, checkIsConnected));

            OutConnectors.Add(new ConnectorData("OUT (OR)", 0, true, true, checkIsConnected));
        }

        protected override Type ConfigControlType => typeof(LogicalOR_ctrl);
    }
}