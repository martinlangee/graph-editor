using System;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Ui;

namespace GraphEditor.Nodes.Bl
{
    public class LogicalOR: NodeDataBase
    {
        public LogicalOR(INodeTypeData nodeTypeData) : base(nodeTypeData)
        {
            InConnectors.Add("IN 1");
            InConnectors.Add("IN 2");
            InConnectors.Add("IN 3");
            InConnectors.Add("IN 4");

            OutConnectors.Add("OUT (OR)");
        }

        protected override Type ConfigControlType => typeof(LogicalOR_ctrl);
    }
}