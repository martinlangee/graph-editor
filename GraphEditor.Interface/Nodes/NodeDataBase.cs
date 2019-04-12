using GraphEditor.Interface.ConfigUi;
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
        private readonly Action<IConnectorData> _onIsActiveChanged;
        private readonly Func<IConnectorData, bool> _canBeDeactivated;
        private readonly Assembly _executingAssembly;
        private string _name;
        
        protected NodeDataBase(IBaseNodeTypeData nodeTypeData, Action<IConnectorData> onIsActiveChanged, Func<IConnectorData, bool> canBeDeactivated, Assembly executingAssembly)
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

        protected void CreateConnector<T>(string name, int index, bool isOutBound, T type, byte[] icon = null)
        {
            (isOutBound ? Outs : Ins).Add(new ConnectorData<T>(name, index, isOutBound, _onIsActiveChanged, _canBeDeactivated, type, icon));
        }

        protected byte[] LoadGraphic(string resourcePath)
        {
            return Helper.LoadGraphicFromResource(resourcePath, _executingAssembly);
        }

        protected byte[] LoadConnIcon(string nodeType, int index, bool isOutBound)
        {
            return LoadGraphic(string.Concat($"/{nodeType}/{nodeType}_", isOutBound ? "out" : "in", $"{index}.png"));
        }

        public IBaseNodeTypeData TypeData { get; }

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
            Id = nodeXml.Attribute("Id").Value;
            Name = nodeXml.Attribute("Name").Value;

            var inpsXml = nodeXml.Element("Inputs").Elements().ToList();
            Ins.For((inp, i) => inp.IsActive = bool.Parse(inpsXml[i].Attribute("Active").Value));

            var outpsXml = nodeXml.Element("Outputs").Elements().ToList();
            Outs.For((outp, i) => outp.IsActive = bool.Parse(outpsXml[i].Attribute("Active").Value));

            LoadTypeSpecificData(nodeXml.Element("Specific"));
        }
        protected abstract void LoadTypeSpecificData(XElement parentXml);

        public void SaveToXml(XElement parentXml)
        {
            parentXml.SetAttributeValue("Id", Id);
            parentXml.SetAttributeValue("Name", Name);
            parentXml.SetAttributeValue("Type", Type);

            var connXml = new XElement("Inputs");

            Ins.For((connData, i) =>
            {
                var inpXml = new XElement("Slot");
                inpXml.SetAttributeValue("Name", connData.Name);
                inpXml.SetAttributeValue("Index", i);
                inpXml.SetAttributeValue("Active", connData.IsActive);
                connXml.Add(inpXml);
            });
            parentXml.Add(connXml);

            connXml = new XElement("Outputs");

            Outs.For((connData, i) =>
            {
                var outpXml = new XElement("Slot");
                outpXml.SetAttributeValue("Name", connData.Name);
                outpXml.SetAttributeValue("Index", i);
                outpXml.SetAttributeValue("Active", connData.IsActive);
                connXml.Add(outpXml);
            });
            parentXml.Add(connXml);

            var specXml = new XElement("Specific");
            SaveTypeSpecificData(specXml);

            parentXml.Add(specXml);
        }
        protected abstract void SaveTypeSpecificData(XElement parentXml);
    }
}
