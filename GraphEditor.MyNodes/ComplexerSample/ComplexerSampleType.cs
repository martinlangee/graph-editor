using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.MyNodes.ComplexerSample
{
    public class ComplexerSampleType : NodeTypeDataBase
    {
        public ComplexerSampleType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Complex Sample Node";
            Description = "Node representing a complex sample node";
        }

        protected override Type NodeType => typeof(ComplexerSample);
    }
}
