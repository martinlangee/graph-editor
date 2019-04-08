using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace GraphEditor.Nodes.ViewModel
{
    public abstract class NodeDataBase : BaseNotification, INodeData
    {
        private string _name;

        public NodeDataBase(INodeTypeData nodeTypeData, Action<IConnectorData, bool> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
        {
            TypeData = nodeTypeData;

            Id = Guid.NewGuid().ToString();
            Name = TypeData.NextNewName;

            Ins = new ObservableCollection<IConnectorData>();
            Outs = new ObservableCollection<IConnectorData>();
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
            Id = nodeXml.Attribute("Id").Value;
            Name = nodeXml.Attribute("Name").Value;

            var inpsXml = nodeXml.Element("Inputs").Elements().ToList();
            Ins.For((inp, i) => inp.IsActive = bool.Parse(inpsXml[i].Attribute("Active").Value));

            var outpsXml = nodeXml.Element("Outputs").Elements().ToList();
            Outs.For((outp, i) => outp.IsActive = bool.Parse(outpsXml[i].Attribute("Active").Value));
            outpsXml.For((outXml, i) => Outs[i].IsActive = bool.Parse(outXml.Attribute("Active").Value));

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
                var inpXml = new XElement("In");
                inpXml.SetAttributeValue("Name", connData.Name);
                inpXml.SetAttributeValue("Index", i);
                inpXml.SetAttributeValue("Active", connData.IsActive);
                connXml.Add(inpXml);
            });
            parentXml.Add(connXml);

            connXml = new XElement("Outputs");

            Outs.For((connData, i) =>
            {
                var outpXml = new XElement("Out");
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
