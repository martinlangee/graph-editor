using System;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Ui;

namespace GraphEditor.Nodes.ViewModel
{
    public class LogicalXOR : NodeDataBase
    {
        public LogicalXOR(INodeTypeData nodeTypeData, Func<IConnectorData, bool> checkIsConnected) 
            : base(nodeTypeData, checkIsConnected)
        {
            InConnectors.Add(new ConnectorData("IN 1", 0, false, true, checkIsConnected));
            InConnectors.Add(new ConnectorData("IN 1", 1, false, true, checkIsConnected));

            OutConnectors.Add(new ConnectorData("OUT (XOR)", 0, true, true, checkIsConnected));
        }

        protected override Type ConfigControlType => typeof(LogicalXOR_ctrl);
    }
}