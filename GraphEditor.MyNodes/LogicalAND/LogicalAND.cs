using System;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Utils;

namespace GraphEditor.MyNodes.LogicalAND
{
    public class LogicalAND : NodeDataBase
    {
        public LogicalAND(INodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("IN 1", 0, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 2", 1, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 3", 2, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 4", 3, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 5", 4, false, SignalType.Red, Colors.Red.ToUint());

            CreateConnector("AND", 0, true, SignalType.Red, Colors.Red.ToUint());
        }

        protected override Type ConfigControlType => typeof(LogicalANDControl);

        protected override void LoadTypeSpecificData(XElement parentXml)
        {

        }

        protected override void SaveTypeSpecificData(XElement parentXml)
        {

        }
    }
}