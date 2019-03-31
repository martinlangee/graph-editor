namespace GraphEditor.Nodes.Bl.Nodes
{
    public class LogicalAND: NodeDataBase
    {
        public LogicalAND(): base()
        {
            InConnectors.Add("IN 1");
            InConnectors.Add("IN 2");
            InConnectors.Add("IN 3");

            OutConnectors.Add("OUT (AND)");
        }
    }
}