using GraphEditor.Nodes.Types;
using System;

namespace GraphEditor.Nodes.LogicalAND
{
    public class LogicalANDType : NodeTypeDataBase
    {
        public LogicalANDType() : base()
        {
            Name = "Logical AND";
            Description = "Node representing a logical AND operation";
        }

        protected override Type NodeType => typeof(LogicalAND);
    }
}
