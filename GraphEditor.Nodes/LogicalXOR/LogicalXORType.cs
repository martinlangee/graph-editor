using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.MyNodes.LogicalXOR
{
    public class LogicalXORType : NodeTypeDataBase
    {
        public LogicalXORType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Logical XOR";
            Description = "Node representing a logical XOR operation";
        }

        protected override Type NodeType => typeof(LogicalXOR);
    }
}
