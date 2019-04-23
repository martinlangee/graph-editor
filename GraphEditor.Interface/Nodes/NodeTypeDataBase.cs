#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

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
            var resPath = $"{nodeType.Name}/{nodeType.Name}{suffix}.png";

            return Helper.LoadGraphicFromResource(resPath, _executingAssembly);
        }

        public string Type => NodeType.Name;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public byte[] Icon { get; protected set; }

        public byte[] Image { get; protected set; }

        protected abstract Type NodeType { get; }

        public INodeData CreateNode(Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
        {
            return Activator.CreateInstance(NodeType, this, onActiveChanged, canBeDeactivated) as INodeData;
        }
    }
}
