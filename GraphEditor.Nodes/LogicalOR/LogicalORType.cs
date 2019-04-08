using GraphEditor.Nodes.Types;
using System;

namespace GraphEditor.Nodes.LogicalOR
{
    public class LogicalORType : NodeTypeDataBase
    {
        public LogicalORType() : base()
        {
            Name = "Logical OR";
            Description = "Node representing a logical OR operation";
        }

        protected override Type NodeType => typeof(LogicalOR);
    }
}
