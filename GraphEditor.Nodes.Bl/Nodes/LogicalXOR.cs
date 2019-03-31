namespace GraphEditor.Nodes.Bl.Nodes
{
    public class LogicalXOR: NodeDataBase
    {
        public LogicalXOR(): base()
        {
            InConnectors.Add("IN 1");
            InConnectors.Add("IN 2");

            OutConnectors.Add("OUT (XOR)");
        }
    }
}