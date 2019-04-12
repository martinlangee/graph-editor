using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;


namespace GraphEditor.MyNodes.ComplexerSample
{
    public class ComplexerSample : NodeDataBase
    {
        public ComplexerSample(IBaseNodeTypeData nodeTypeData, Action<IBaseConnectorData> onActiveChanged, Func<IBaseConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("Water input", 0, false, SignalType.Red);
            CreateConnector("Stop", 1, false, SignalType.Red);
            CreateConnector("Start", 2, false, SignalType.Red);
            CreateConnector("Reduce", 3, false, SignalType.Red, LoadConnIcon(nodeTypeData.Type, 3, false));
            CreateConnector("Increase", 4, false, SignalType.Red);
            CreateConnector("Reset", 5, false, SignalType.Red);

            CreateConnector("Water output", 0, true, SignalType.Red);
            CreateConnector("Forward active", 1, true, SignalType.Red);
            CreateConnector("Backward active", 2, true, SignalType.Green, LoadConnIcon(nodeTypeData.Type, 2, true));
            CreateConnector("Startup active", 3, true, SignalType.Green, LoadConnIcon(nodeTypeData.Type, 3, true));
            CreateConnector("Wait time active", 4, true, SignalType.Red);
            CreateConnector("Warning", 5, true, SignalType.Red);
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