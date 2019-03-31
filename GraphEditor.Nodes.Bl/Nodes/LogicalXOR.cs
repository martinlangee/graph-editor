using GraphEditor.Interfaces.Nodes;

namespace GraphEditor.Nodes.Bl.Nodes
{
    public class LogicalXOR: NodeDataBase
    {
        public LogicalXOR(INodeTypeData nodeTypeData) : base(nodeTypeData)
        {
            InConnectors.Add("IN 1");
            InConnectors.Add("IN 2");

            OutConnectors.Add("OUT (XOR)");
        }
    }
}