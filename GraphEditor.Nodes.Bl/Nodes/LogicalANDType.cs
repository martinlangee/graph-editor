using System;

namespace GraphEditor.Nodes.Bl.Nodes
{
    public class LogicalANDType: NodeTypeDataBase
    {
        public LogicalANDType()
        {
            Name = "Logical AND";
            Description = "Node representing a logical AND operation";
        }

        protected override Type NodeType => typeof(LogicalAND);
    }
}
