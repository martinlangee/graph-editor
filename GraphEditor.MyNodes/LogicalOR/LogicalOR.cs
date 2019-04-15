using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;
using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Utils;

namespace GraphEditor.MyNodes.LogicalOR
{
    public class LogicalOR : NodeDataBase
    {
        bool _outputInverted;
        private readonly IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();

        public LogicalOR(INodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("IN 1", 0, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 2", 1, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 3", 2, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 4", 3, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("IN 5", 4, false, SignalType.Red, Colors.Red.ToUint());

            CreateConnector("OR", 0, true, SignalType.Red, Colors.Red.ToUint());
        }

        protected override Type ConfigControlType => typeof(LogicalORControl);

        protected override void LoadTypeSpecificData(XElement specificXml)
        {
            var values = _xmlClasses.GetParamValues(specificXml, nameof(OutputInverted));

            OutputInverted = values.FirstOrDefault(kvp => kvp.Key == nameof(OutputInverted)).Value.ToLower() == bool.TrueString.ToLower();
        }

        protected override void SaveTypeSpecificData(XElement specificXml)
        {
            var param = new XElement(_xmlClasses.Param);
            param.SetAttributeValue(_xmlClasses.Id, nameof(OutputInverted));
            param.SetAttributeValue(_xmlClasses.Name, "Output inverted");
            param.SetAttributeValue(_xmlClasses.Value, OutputInverted);
            specificXml.Add(param);
        }

        public bool OutputInverted { get => _outputInverted; set => SetProperty<LogicalOR, bool>(ref _outputInverted, value, nameof(OutputInverted)); }
    }
}