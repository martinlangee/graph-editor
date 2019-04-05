using GraphEditor.Nodes.ViewModel;
using System;

namespace GraphEditor.Nodes.Types
{
    public class LogicalXOR_Type : NodeTypeDataBase
    {
        public LogicalXOR_Type() : base()
        {
            Name = "Logical XOR";
            Description = "Node representing a logical XOR operation";
        }

        protected override Type NodeType => typeof(LogicalXOR);
    }
}
