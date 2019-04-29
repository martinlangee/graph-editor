#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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
