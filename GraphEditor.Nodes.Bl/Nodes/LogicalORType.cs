using System;

namespace GraphEditor.Nodes.Bl.Nodes
{
    public class LogicalORType: NodeTypeDataBase
    {
        public LogicalORType(): base()
        {
            Name = "Logical OR";
            Description = "Node representing a logical OR operation";
        }

        protected override Type NodeType => typeof(LogicalOR);
    }
}
