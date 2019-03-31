using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.Nodes.Bl
{
    public abstract class NodeTypeDataBase : INodeTypeData
    {
        public NodeTypeDataBase()
        {
            Name = "<not set>";
            Description = "<not set>";
        }

        public string Type => GetType().Name;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public byte[] Image { get; protected set; }

        public virtual UserControl ConfigUi => null;

        public virtual bool CanConnectToIn(INodeTypeData otherNode, int otherOutIndex, int myInIndex) => true;

        public virtual bool CanConnectToOut(INodeTypeData otherNode, int otherInIndex, int myOutIndex) => true;

        protected abstract Type NodeType { get; }

        public INodeData CreateNode()
        {
            return Activator.CreateInstance(NodeType) as INodeData;
        }
    }
}
