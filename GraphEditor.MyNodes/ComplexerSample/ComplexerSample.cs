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
using System.Windows.Media;
using System.Xml.Linq;
using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Utils;

namespace GraphEditor.MyNodes.ComplexerSample
{
    public class ComplexerSample : NodeDataBase
    {
        private bool _outputsInverted;
        private double _filterTime;
        private readonly IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();

        public ComplexerSample(INodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated): 
            base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("Water input", 0, false, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("Stop", 1, false, SignalType.Blue, Colors.Blue.ToUint());
            CreateConnector("Start", 2, false, SignalType.Orange, Colors.Orange.ToUint());
            CreateConnector("Reduce", 3, false, SignalType.Green, Colors.Green.ToUint(), LoadConnIcon(nodeTypeData.Type, 3, false));
            CreateConnector("Invert", 4, false, SignalType.Red);
            CreateConnector("Reset", 5, false, SignalType.Green, LoadConnIcon(nodeTypeData.Type, 3, false));

            CreateConnector("Water output", 0, true, SignalType.Blue, Colors.Blue.ToUint());
            CreateConnector("Forward active", 1, true, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("Backward active", 2, true, SignalType.Green, Colors.Green.ToUint(), LoadConnIcon(nodeTypeData.Type, 2, true));
            CreateConnector("Startup active", 3, true, SignalType.Green, Colors.Green.ToUint(), LoadConnIcon(nodeTypeData.Type, 3, true));
            CreateConnector("Wait time active", 4, true, SignalType.Red, Colors.Red.ToUint());
            CreateConnector("Warning", 5, true, SignalType.Orange, Colors.Orange.ToUint());
        }

        protected override Type ConfigControlType => typeof(ComplexerSampleControl);

        protected override void LoadTypeSpecificData(XElement specificXml)
        {
            var values = _xmlClasses.GetParamValues(specificXml, nameof(OutputsInverted), nameof(FilterTime));

            OutputsInverted = values.FirstOrDefault(kvp => kvp.Key == nameof(OutputsInverted)).Value.ToLower() == bool.TrueString.ToLower();
            FilterTime = double.Parse(values.FirstOrDefault(kvp => kvp.Key == nameof(FilterTime)).Value);
        }

        protected override void SaveTypeSpecificData(XElement specificXml)
        {
            var param = new XElement(_xmlClasses.Param);
            param.SetAttributeValue(_xmlClasses.Id, nameof(OutputsInverted));
            param.SetAttributeValue(_xmlClasses.Name, "Outputs inverted");
            param.SetAttributeValue(_xmlClasses.Value, OutputsInverted);
            specificXml.Add(param);

            param = new XElement(_xmlClasses.Param);
            param.SetAttributeValue(_xmlClasses.Id, nameof(FilterTime));
            param.SetAttributeValue(_xmlClasses.Name, "Filter time [ms]");
            param.SetAttributeValue(_xmlClasses.Value, FilterTime);
            specificXml.Add(param);
        }

        public bool OutputsInverted { get => _outputsInverted; set => SetProperty<ComplexerSample, bool>(ref _outputsInverted, value, nameof(OutputsInverted)); }

        public double FilterTime { get => _filterTime; set => SetProperty<ComplexerSample, double>(ref _filterTime, value, nameof(FilterTime)); }
    }
}