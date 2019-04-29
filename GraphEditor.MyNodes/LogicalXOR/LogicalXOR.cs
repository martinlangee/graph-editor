#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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