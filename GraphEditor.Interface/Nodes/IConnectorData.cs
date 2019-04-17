using System;
using System.Windows.Media;

namespace GraphEditor.Interface.Nodes
{
    public interface IConnectorData
    {
        string Name { get; }

        int Index { get; }

        bool IsOutBound { get; }

        bool IsActive { get; set; }

        byte[] Icon { get; set;  }

        event Action IconChanged;

        object Type { get; }

        uint Color { get; }
    }
}
