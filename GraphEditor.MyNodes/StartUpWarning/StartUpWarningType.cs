using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.MyNodes.StartUpWarning
{
    public class StartUpWarningType : NodeTypeDataBase
    {
        public StartUpWarningType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Start-Up Warning";
            Description = "Node representing a start-up warning element";
        }

        protected override Type NodeType => typeof(StartUpWarning);
    }
}
