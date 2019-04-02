using System.Windows.Controls;

namespace GraphEditor.Interfaces.Nodes
{
    public interface INodeTypeData
    {
        string Type { get; }

        string Name { get; }

        string Description { get; }

        byte[] Icon { get; }

        byte[] Image { get; }

        bool CanConnectToIn(INodeTypeData otherNode, int otherOutIndex, int myInIndex);

        bool CanConnectToOut(INodeTypeData otherNode, int otherInIndex, int myOutIndex);

        INodeData CreateNode();
    }
}