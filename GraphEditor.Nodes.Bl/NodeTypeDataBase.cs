using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;
using System.IO;

namespace GraphEditor.Nodes.Bl
{
    public abstract class NodeTypeDataBase : INodeTypeData
    {
        public NodeTypeDataBase()
        {
            Name = "<not set>";
            Description = "<not set>";

            var img = System.Drawing.Image.FromFile("C:\\Git\\GraphEditor\\GraphEditor.Nodes.Bl\\Nodes\\LogicalAND.png");
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                Image = ms.ToArray();
            }
        }

        public string Type => GetType().Name;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public byte[] Icon { get; protected set; }

        public byte[] Image { get; protected set; }

        public virtual UserControl ConfigUi => null;

        public virtual bool CanConnectToIn(INodeTypeData otherNode, int otherOutIndex, int myInIndex) => myInIndex % 2 == 0;

        public virtual bool CanConnectToOut(INodeTypeData otherNode, int otherInIndex, int myOutIndex) => myOutIndex % 2 == 1;

        protected abstract Type NodeType { get; }

        public INodeData CreateNode()
        {
            return Activator.CreateInstance(NodeType, this) as INodeData;
        }
    }
}
