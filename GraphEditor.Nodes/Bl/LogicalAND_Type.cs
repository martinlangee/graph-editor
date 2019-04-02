using System;

namespace GraphEditor.Nodes.Bl
{
    public class LogicalAND_Type: NodeTypeDataBase
    {
        public LogicalAND_Type(): base()
        {
            Name = "Logical AND";
            Description = "Node representing a logical AND operation";
        }

        protected override Type NodeType => typeof(LogicalAND);
    }
}
