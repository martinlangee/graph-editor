using System;
using System.Xml.Linq;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Base;
using GraphEditor.Nodes.LogicalXOR;

namespace GraphEditor.Nodes.StartUpWarning
{
    public class StartUpWarning : NodeDataBase
    {
        public StartUpWarning(INodeTypeData nodeTypeData, Action<IConnectorData, bool> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated)
        {
            Ins.Add(new ConnectorData("Control input", 0, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("Locking", 1, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("Stop", 2, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("Tap forward", 3, false, true, onActiveChanged, canBeDeactivated, LoadConnIcon(nodeTypeData.Type, 3, false)));
            Ins.Add(new ConnectorData("Tap backward", 4, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("Reset", 4, false, true, onActiveChanged, canBeDeactivated));

            Outs.Add(new ConnectorData("Release", 0, true, true, onActiveChanged, canBeDeactivated));
            Outs.Add(new ConnectorData("Forward active", 1, true, true, onActiveChanged, canBeDeactivated));
            Outs.Add(new ConnectorData("Backward active", 2, true, true, onActiveChanged, canBeDeactivated, LoadConnIcon(nodeTypeData.Type, 2, true)));
            Outs.Add(new ConnectorData("Startup active", 3, true, true, onActiveChanged, canBeDeactivated, LoadConnIcon(nodeTypeData.Type, 3, true)));
            Outs.Add(new ConnectorData("Wait time active", 4, true, true, onActiveChanged, canBeDeactivated));
            Outs.Add(new ConnectorData("Warning", 5, true, true, onActiveChanged, canBeDeactivated));
        }

        private static byte[] LoadConnIcon(string nodeType, int index, bool isOutBound)
        {
            return LoadGraphic(string.Concat($"/{nodeType}/{nodeType}_", isOutBound ? "out" : "in", $"{index}.png"));
        }

        protected override Type ConfigControlType => typeof(LogicalXORControl);

        protected override void LoadTypeSpecificData(XElement parentXml)
        {

        }

        protected override void SaveTypeSpecificData(XElement parent)
        {

        }
    }
}