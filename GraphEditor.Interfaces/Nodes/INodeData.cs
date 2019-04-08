using GraphEditor.Interfaces.ConfigUi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace GraphEditor.Interfaces.Nodes
{
    public interface INodeData
    {
        INodeTypeData TypeData { get; }

        string Id { get; }

        string Type { get; }

        string Name { get; set; }

        IList<IConnectorData> Ins { get; }

        IList<IConnectorData> Outs { get; }

        INodeConfigUi CreateConfigUi();

        void SaveToXml(XElement parent);

        void LoadFromXml(XElement parent);
    }
}