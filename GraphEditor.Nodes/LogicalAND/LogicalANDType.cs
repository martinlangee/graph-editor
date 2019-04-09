using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.Nodes.LogicalAND
{
    public class LogicalANDType : NodeTypeDataBase
    {
        public LogicalANDType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Logical AND";
            Description = "Node representing a logical AND operation";
        }

        protected override Type NodeType => typeof(LogicalAND);
    }
}
