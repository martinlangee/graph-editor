using GraphEditor.Interfaces.ConfigUi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace GraphEditor.Interfaces.Nodes
{
    public interface INodeData
    {
        INodeTypeData TypeData { get; }

        string Id { get; }

        string Type { get; }

        string Name { get; set; }

        IList<IConnectorData> InConnectors { get; }

        IList<IConnectorData> OutConnectors { get; }

        INodeConfigUi CreateConfigUi();
    }
}