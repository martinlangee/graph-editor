using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;

namespace GraphEditor.MyNodes.StartUpWarning
{
    public class StartUpWarning : NodeDataBase
    {
        public StartUpWarning(INodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("Control input", 0, false);
            CreateConnector("Locking", 1, false);
            CreateConnector("Stop", 2, false);
            CreateConnector("Tap forward", 3, false, LoadConnIcon(nodeTypeData.Type, 3, false));
            CreateConnector("Tap backward", 4, false);
            CreateConnector("Reset", 5, false);

            CreateConnector("Release", 0, true);
            CreateConnector("Forward active", 1, true);
            CreateConnector("Backward active", 2, true, LoadConnIcon(nodeTypeData.Type, 2, true));
            CreateConnector("Startup active", 3, true, LoadConnIcon(nodeTypeData.Type, 3, true));
            CreateConnector("Wait time active", 4, true);
            CreateConnector("Warning", 5, true);
        }

        private byte[] LoadConnIcon(string nodeType, int index, bool isOutBound)
        {
            return LoadGraphic(string.Concat($"/{nodeType}/{nodeType}_", isOutBound ? "out" : "in", $"{index}.png"));
        }

        protected override Type ConfigControlType => typeof(StartUpWarningControl);

        protected override void LoadTypeSpecificData(XElement parentXml)
        {

        }

        protected override void SaveTypeSpecificData(XElement parent)
        {

        }
    }
}