using GraphEditor.Interfaces.Nodes;

namespace GraphEditor.Nodes.Bl.Nodes
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
    }
}