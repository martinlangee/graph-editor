using System.Collections.Generic;
using System.Windows.Controls;

namespace GraphEditor.Interfaces.Nodes
{
    public interface INodeData
    {
        string Id { get; }

        string Type { get; }

        IList<string> InConnectors { get; }

        IList<string> OutConnectors { get; }
    }
}