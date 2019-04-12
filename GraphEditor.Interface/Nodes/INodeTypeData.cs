using System;
using System.Collections.Generic;

namespace GraphEditor.Interface.Nodes
{
    public interface IBaseNodeTypeData
    {
        string Type { get; }

        string Name { get; }

        string NextNewName { get; }

        string Description { get; }

        byte[] Icon { get; }

        byte[] Image { get; }

        bool CanConnectToIn(IBaseNodeTypeData otherNode, int otherOutIndex, int myInIndex);

        bool CanConnectToOut(IBaseNodeTypeData otherNode, int otherInIndex, int myOutIndex);

        INodeData CreateNode(Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated);
    }
}