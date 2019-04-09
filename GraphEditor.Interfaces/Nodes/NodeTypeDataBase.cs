using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;
using GraphEditor.Interface.Utils;

namespace GraphEditor.Interface.Nodes
{
    public abstract class NodeTypeDataBase : INodeTypeData
    {
        private readonly Assembly _executingAssembly;
        protected NodeTypeDataBase(Assembly executingAssembly)
        {
            _executingAssembly = executingAssembly;

            Name = "<not set>";
            Description = "<not set>";

            Image = LoadGraphic(NodeType);
            Icon = LoadGraphic(NodeType, "ico");
        }

        private int _newIndex = 1;

        public string NextNewName
        {
            get { return $"{Name} {_newIndex++}"; }
        }

        private byte[] LoadGraphic(Type nodeType, string suffix = "")
        {
            suffix = string.IsNullOrEmpty(suffix) ? "" : $"_{suffix}";
            var resPath = $"/{nodeType.Name}/{nodeType.Name}{suffix}.png";

            return Helper.LoadGraphicFromResource(resPath, _executingAssembly);
        }

        public string Type => NodeType.Name;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public byte[] Icon { get; protected set; }

        public byte[] Image { get; protected set; }

        public virtual bool CanConnectToIn(INodeTypeData otherNode, int otherOutIndex, int myInIndex) => myInIndex % 2 == 0;  // TODO: CanConnectToIn zum Testen

        public virtual bool CanConnectToOut(INodeTypeData otherNode, int otherInIndex, int myOutIndex) => myOutIndex % 2 == 1;  // TODO: CanConnectToOut zum Testen

        protected abstract Type NodeType { get; }

        public INodeData CreateNode(Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
        {
            return Activator.CreateInstance(NodeType, this, onActiveChanged, canBeDeactivated) as INodeData;
        }
    }
}
