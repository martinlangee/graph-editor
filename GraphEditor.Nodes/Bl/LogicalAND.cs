using System;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Ui;

namespace GraphEditor.Nodes.Bl
{
    public class LogicalAND: NodeDataBase
    {
        public LogicalAND(INodeTypeData nodeTypeData) : base(nodeTypeData)
        {
            InConnectors.Add("IN 1");
            InConnectors.Add("IN 2");
            InConnectors.Add("IN 3");
            InConnectors.Add("IN 4");
            InConnectors.Add("IN 5");

            OutConnectors.Add("OUT 1 (AND)");
            OutConnectors.Add("OUT 2 (AND)");
            OutConnectors.Add("OUT 3 (AND)");
            OutConnectors.Add("OUT 4 (AND)");
            OutConnectors.Add("OUT 5 (AND)");
        }

        protected override Type ConfigControlType => typeof(LogicalAND_ctrl);
    }
}