namespace GraphEditor.Nodes.Bl.Nodes
{
    public class LogicalOR: NodeDataBase
    {
        public LogicalOR(): base()
        {
            InConnectors.Add("IN 1");
            InConnectors.Add("IN 2");
            InConnectors.Add("IN 3");
            InConnectors.Add("IN 4");

            OutConnectors.Add("OUT (OR)");
        }
    }
}