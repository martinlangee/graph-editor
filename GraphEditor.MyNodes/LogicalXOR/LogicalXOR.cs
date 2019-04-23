#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Serialization;

namespace GraphEditor.MyNodes.LogicalXOR
{
    public class LogicalXOR : NodeDataBase
    {
        bool _outputInverted;
        private readonly IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();

        public LogicalXOR(INodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("IN 1", 0, false, SignalType.Red);
            CreateConnector("IN 2", 1, false, SignalType.Red);
            CreateConnector("IN 3", 2, false, SignalType.Red);
            CreateConnector("IN 4", 3, false, SignalType.Red);
            CreateConnector("IN 5", 4, false, SignalType.Red);

            CreateConnector("XOR", 0, true, SignalType.Red);
        }

        protected override Type ConfigControlType => typeof(LogicalXORControl);

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

        public bool OutputInverted
        {
            get => _outputInverted;
            set => SetProperty<LogicalXOR, bool>(ref _outputInverted, value, nameof(OutputInverted),
                (nodeData, val) =>
                {
                    Outs[0].Icon = val ? LoadGraphic(nameof(LogicalXOR), $"{nameof(LogicalXOR)}_inverted.png") : null;
                });
        }
    }
}