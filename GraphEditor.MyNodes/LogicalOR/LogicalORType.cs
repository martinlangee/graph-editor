using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.MyNodes.LogicalOR
{
    public class LogicalORType : NodeTypeDataBase
    {
        public LogicalORType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Logical OR";
            Description = "Node representing a logical OR operation";
        }

        protected override Type NodeType => typeof(LogicalOR);
    }
}
