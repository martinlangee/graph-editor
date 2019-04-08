using System;
using System.Xml.Linq;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Base;

namespace GraphEditor.Nodes.LogicalOR
{
    public class LogicalOR : NodeDataBase
    {
        public LogicalOR(INodeTypeData nodeTypeData, Action<IConnectorData, bool> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated)
        {
            Ins.Add(new ConnectorData("IN 1", 0, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("IN 2", 1, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("IN 3", 2, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("IN 4", 3, false, true, onActiveChanged, canBeDeactivated));

            Outs.Add(new ConnectorData("OR", 0, true, true, onActiveChanged, canBeDeactivated));
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