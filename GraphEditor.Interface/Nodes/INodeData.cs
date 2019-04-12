using GraphEditor.Interface.ConfigUi;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GraphEditor.Interface.Nodes
{
    public interface INodeData
    {
        IBaseNodeTypeData TypeData { get; }

        string Id { get; }

        string Type { get; }

        string Name { get; set; }

        IList<IConnectorData> Ins { get; }

        IList<IConnectorData> Outs { get; }

        INodeConfigUi CreateConfigUi();

        void SaveToXml(XElement parentXml);

        void LoadFromXml(XElement nodeXml);
    }
}