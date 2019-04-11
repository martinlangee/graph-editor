using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;

namespace GraphEditor.MyNodes.LogicalOR
{
    public class LogicalOR : NodeDataBase
    {
        public LogicalOR(IBaseNodeTypeData nodeTypeData, Action<IBaseConnectorData> onActiveChanged, Func<IBaseConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("IN 1", 0, false, SignalType.Digital);
            CreateConnector("IN 2", 1, false, SignalType.Digital);
            CreateConnector("IN 3", 2, false, SignalType.Digital);
            CreateConnector("IN 4", 3, false, SignalType.Digital);
            CreateConnector("IN 5", 4, false, SignalType.Digital);

            CreateConnector("OR", 0, true, SignalType.Digital);
        }

        protected override Type ConfigControlType => typeof(LogicalORControl);

        protected override void LoadTypeSpecificData(XElement parentXml)
        {

        }

        protected override void SaveTypeSpecificData(XElement parent)
        {

        }
    }
}