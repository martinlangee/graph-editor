using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;


namespace GraphEditor.MyNodes.ComplexerSample
{
    public class ComplexerSample : NodeDataBase
    {
        public ComplexerSample(IBaseNodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("Water input", 0, false, SignalType.Red);
            CreateConnector("Stop", 1, false, SignalType.Blue);
            CreateConnector("Start", 2, false, SignalType.Orange);
            CreateConnector("Reduce", 3, false, SignalType.Green, LoadConnIcon(nodeTypeData.Type, 3, false));
            CreateConnector("Invert", 4, false, SignalType.Red);
            CreateConnector("Reset", 5, false, SignalType.Green);

            CreateConnector("Water output", 0, true, SignalType.Blue);
            CreateConnector("Forward active", 1, true, SignalType.Red);
            CreateConnector("Backward active", 2, true, SignalType.Green, LoadConnIcon(nodeTypeData.Type, 2, true));
            CreateConnector("Startup active", 3, true, SignalType.Green, LoadConnIcon(nodeTypeData.Type, 3, true));
            CreateConnector("Wait time active", 4, true, SignalType.Red);
            CreateConnector("Warning", 5, true, SignalType.Orange);
        }

        protected override Type ConfigControlType => typeof(ComplexerSampleControl);

        protected override void LoadTypeSpecificData(XElement parentXml)
        {

        }

        protected override void SaveTypeSpecificData(XElement parent)
        {

        }
    }
}