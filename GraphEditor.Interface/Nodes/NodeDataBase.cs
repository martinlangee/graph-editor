using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Container;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace GraphEditor.Interface.Nodes
{
    public abstract class NodeDataBase: BaseNotification, INodeData
    {
        private const uint DefaultConnectorColor = 0xFF6495FD;  // == Colors.CornflowerBlue

        private readonly Action<IConnectorData> _onIsActiveChanged;
        private readonly Func<IConnectorData, bool> _canBeDeactivated;
        private readonly Assembly _executingAssembly;
        private string _name;
        private readonly IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();

        protected NodeDataBase(INodeTypeData nodeTypeData, Action<IConnectorData> onIsActiveChanged, Func<IConnectorData, bool> canBeDeactivated, Assembly executingAssembly)
        {
            TypeData = nodeTypeData;
            _onIsActiveChanged = onIsActiveChanged;
            _canBeDeactivated = canBeDeactivated;
            _executingAssembly = executingAssembly;

            Id = Guid.NewGuid().ToString();
            Name = TypeData.NextNewName;

            Ins = new ObservableCollection<IConnectorData>();
            Outs = new ObservableCollection<IConnectorData>();
        }

        protected void CreateConnector<T>(string name, int index, bool isOutBound, T type, uint color = DefaultConnectorColor, byte[] icon = null)
        {
            (isOutBound ? Outs : Ins).Add(new ConnectorData<T>(name, index, isOutBound, _onIsActiveChanged, _canBeDeactivated, type, color, icon));
        }

        protected void CreateConnector<T>(string name, int index, bool isOutBound, T type, byte[] icon)
        {
            (isOutBound ? Outs : Ins).Add(new ConnectorData<T>(name, index, isOutBound, _onIsActiveChanged, _canBeDeactivated, type, DefaultConnectorColor, icon));
        }

        private byte[] LoadGraphic(string resourceName)
        {
            return Helper.LoadGraphicFromResource(resourceName, _executingAssembly);
        }

        protected byte[] LoadGraphic(string nodeType, string resourceName)
        {
            return Helper.LoadGraphicFromResource($"{nodeType}/{resourceName}", _executingAssembly);
        }

        protected byte[] LoadConnIcon(string nodeType, int index, bool isOutBound)
        {
            return LoadGraphic(string.Concat($"{nodeType}/{nodeType}_", isOutBound ? "out" : "in", $"{index}.png"));
        }

        public INodeTypeData TypeData { get; }

        public string Id { get; private set; }

        public string Type => GetType().Name;

        public IList<IConnectorData> Ins { get; private set; }

        public IList<IConnectorData> Outs { get; private set; }

        public string Name { get => _name; set => SetProperty<NodeDataBase, string>(ref _name, value, nameof(Name)); }

        protected abstract Type ConfigControlType { get; }

        public INodeConfigUi CreateConfigUi()
        {
            return Activator.CreateInstance(ConfigControlType, this) as INodeConfigUi;
        }

        public void LoadFromXml(XElement nodeXml)
        {
            Id = nodeXml.Attribute(_xmlClasses.Id).Value;
            Name = nodeXml.Attribute(_xmlClasses.Name).Value;

            var inpsXml = nodeXml.Element(_xmlClasses.Inputs).Elements().ToList();
            Ins.For((inp, i) => inp.IsActive = bool.Parse(inpsXml[i].Attribute(_xmlClasses.Active).Value));

            var outpsXml = nodeXml.Element(_xmlClasses.Outputs).Elements().ToList();
            Outs.For((outp, i) => outp.IsActive = bool.Parse(outpsXml[i].Attribute(_xmlClasses.Active).Value));

            LoadTypeSpecificData(nodeXml.Element(_xmlClasses.Specific));
        }
        protected abstract void LoadTypeSpecificData(XElement parentXml);

        public void SaveToXml(XElement parentXml)
        {
            parentXml.SetAttributeValue(_xmlClasses.Id, Id);
            parentXml.SetAttributeValue(_xmlClasses.Name, Name);
            parentXml.SetAttributeValue(_xmlClasses.Type, Type);

            var connXml = new XElement(_xmlClasses.Inputs);

            Ins.For((connData, i) =>
            {
                var inpXml = new XElement(_xmlClasses.Slot);
                inpXml.SetAttributeValue(_xmlClasses.Name, connData.Name);
                inpXml.SetAttributeValue(_xmlClasses.Index, i);
                inpXml.SetAttributeValue(_xmlClasses.Active, connData.IsActive);
                connXml.Add(inpXml);
            });
            parentXml.Add(connXml);

            connXml = new XElement(_xmlClasses.Outputs);

            Outs.For((connData, i) =>
            {
                var outpXml = new XElement(_xmlClasses.Slot);
                outpXml.SetAttributeValue(_xmlClasses.Name, connData.Name);
                outpXml.SetAttributeValue(_xmlClasses.Index, i);
                outpXml.SetAttributeValue(_xmlClasses.Active, connData.IsActive);
                connXml.Add(outpXml);
            });
            parentXml.Add(connXml);

            var specXml = new XElement(_xmlClasses.Specific);
            SaveTypeSpecificData(specXml);

            parentXml.Add(specXml);
        }
        protected abstract void SaveTypeSpecificData(XElement parentXml);

        public virtual bool CanConnectTo(int formIdx, IConnectorData toConnector)
        {
            return toConnector == null ||
                   toConnector.IsOutBound 
                       ? Ins[formIdx].Type == null || Ins[formIdx].Type.Equals(toConnector.Type) 
                       : Outs[formIdx].Type == null || Outs[formIdx].Type.Equals(toConnector.Type);
        }
    }
}
