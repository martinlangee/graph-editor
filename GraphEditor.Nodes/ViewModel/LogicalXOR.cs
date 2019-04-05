using System;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Ui;

namespace GraphEditor.Nodes.ViewModel
{
    public class LogicalXOR : NodeDataBase
    {
        public LogicalXOR(INodeTypeData nodeTypeData) : base(nodeTypeData)
        {
            InConnectors.Add("IN 1");
            InConnectors.Add("IN 2");

            OutConnectors.Add("OUT (XOR)");
        }

        protected override Type ConfigControlType => typeof(LogicalXOR_ctrl);
    }
}