using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.MyNodes.ComplexerSample
{
    public class ComplexerSampleType : NodeTypeDataBase
    {
        public ComplexerSampleType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Complexer Sampler";
            Description = "Node representing a start-up warning element";
        }

        protected override Type NodeType => typeof(ComplexerSample);
    }
}
