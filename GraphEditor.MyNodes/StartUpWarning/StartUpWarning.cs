using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;

namespace GraphEditor.MyNodes.StartUpWarning
{
    public class StartUpWarning : NodeDataBase
    {
        public StartUpWarning(IBaseNodeTypeData nodeTypeData, Action<IBaseConnectorData> onActiveChanged, Func<IBaseConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("Control input", 0, false, SignalType.Digital);
            CreateConnector("Locking", 1, false, SignalType.Digital);
            CreateConnector("Stop", 2, false, SignalType.Digital);
            CreateConnector("Tap forward", 3, false, SignalType.Digital, LoadConnIcon(nodeTypeData.Type, 3, false));
            CreateConnector("Tap backward", 4, false, SignalType.Digital);
            CreateConnector("Reset", 5, false, SignalType.Digital);

            CreateConnector("Release", 0, true, SignalType.Digital);
            CreateConnector("Forward active", 1, true, SignalType.Digital);
            CreateConnector("Backward active", 2, true, SignalType.Analog, LoadConnIcon(nodeTypeData.Type, 2, true));
            CreateConnector("Startup active", 3, true, SignalType.Analog, LoadConnIcon(nodeTypeData.Type, 3, true));
            CreateConnector("Wait time active", 4, true, SignalType.Digital);
            CreateConnector("Warning", 5, true, SignalType.Digital);
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