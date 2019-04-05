using GraphEditor.Nodes.ViewModel;
using System;

namespace GraphEditor.Nodes.Types
{
    public class LogicalOR_Type : NodeTypeDataBase
    {
        public LogicalOR_Type() : base()
        {
            Name = "Logical OR";
            Description = "Node representing a logical OR operation";
        }

        protected override Type NodeType => typeof(LogicalOR);
    }
}
