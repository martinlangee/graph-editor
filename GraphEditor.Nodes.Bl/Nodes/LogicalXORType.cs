using System;

namespace GraphEditor.Nodes.Bl.Nodes
{
    public class LogicalXORType: NodeTypeDataBase
    {
        public LogicalXORType(): base()
        {
            Name = "Logical XOR";
            Description = "Node representing a logical XOR operation";
        }

        protected override Type NodeType => typeof(LogicalXOR);
    }
}
