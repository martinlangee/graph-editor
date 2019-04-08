using GraphEditor.Nodes.Types;
using System;

namespace GraphEditor.Nodes.StartUpWarning
{
    public class StartUpWarningType : NodeTypeDataBase
    {
        public StartUpWarningType() : base()
        {
            Name = "Start-Up Warning";
            Description = "Node representing a start-up warning element";
        }

        protected override Type NodeType => typeof(StartUpWarning);
    }
}
