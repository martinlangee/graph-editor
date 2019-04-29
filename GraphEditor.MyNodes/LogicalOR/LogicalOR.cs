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

        public bool OutputInverted
        {
            get => _outputInverted;
            set => SetProperty<LogicalOR, bool>(ref _outputInverted, value, nameof(OutputInverted),
                (nodeData, val) =>
                {
                    Outs[0].Icon = val ? LoadGraphic(nameof(LogicalOR), $"{nameof(LogicalOR)}_inverted.png") : null;
                });
        }
    }
}