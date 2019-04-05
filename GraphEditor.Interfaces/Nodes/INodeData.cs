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

        IList<string> InConnectors { get; }

        IList<string> OutConnectors { get; }

        INodeConfigUi CreateConfigUi();

        Func<NotifyCollectionChangedEventArgs, bool> OnInConnectorsChanged { set; }

        Func<NotifyCollectionChangedEventArgs, bool> OnOutConnectorsChanged { set; }
    }
}