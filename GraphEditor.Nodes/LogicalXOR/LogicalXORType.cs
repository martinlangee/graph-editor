using GraphEditor.Nodes.Types;
using System;

namespace GraphEditor.Nodes.LogicalXOR
{
    public class LogicalXORType : NodeTypeDataBase
    {
        public LogicalXORType() : base()
        {
            Name = "Logical XOR";
            Description = "Node representing a logical XOR operation";
        }

        protected override Type NodeType => typeof(LogicalXOR);
    }
}
