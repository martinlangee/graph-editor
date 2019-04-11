using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.MyNodes.LogicalAND
{
    public class LogicalANDType : NodeTypeDataBase
    {
        public LogicalANDType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Logical AND";
            Description = "Node representing a logical AND operation";
        }

        protected override Type NodeType => typeof(LogicalAND);

        protected void CreateConnectors()
        {
            //CreateConnector("IN 1", 0, false, SignalType.Digital);
            //CreateConnector("IN 2", 1, false, SignalType.Digital);
            //CreateConnector("IN 3", 2, false, SignalType.Digital);
            //CreateConnector("IN 4", 3, false, SignalType.Digital);
            //CreateConnector("IN 5", 4, false, SignalType.Digital);

            //CreateConnector("OR", 0, true, SignalType.Digital);

        }

        public override bool CanConnectToIn(IBaseNodeTypeData otherNode, int otherOutIndex, int myInIndex)
        {
            return base.CanConnectToIn(otherNode, otherOutIndex, myInIndex);
        }
    }
}
