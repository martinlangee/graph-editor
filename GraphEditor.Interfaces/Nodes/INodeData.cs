using GraphEditor.Interfaces.ConfigUi;
using System.Collections.Generic;
using System.Windows.Controls;

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
    }
}