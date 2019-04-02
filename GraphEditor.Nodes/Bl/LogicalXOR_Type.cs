using System;

namespace GraphEditor.Nodes.Bl
{
    public class LogicalXOR_Type: NodeTypeDataBase
    {
        public LogicalXOR_Type(): base()
        {
            Name = "Logical XOR";
            Description = "Node representing a logical XOR operation";
        }

        protected override Type NodeType => typeof(LogicalXOR);
    }
}
