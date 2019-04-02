using GraphEditor.Interfaces.Nodes;
using System;
using System.Collections.Generic;

namespace GraphEditor.Nodes
{
    public abstract class NodeDataBase : INodeData
    {
        public NodeDataBase(INodeTypeData nodeTypeData)
        {
            TypeData = nodeTypeData;

            Id = Guid.NewGuid().ToString();
            Name = Type;

            InConnectors = new List<string>();
            OutConnectors = new List<string>();
        }

        public INodeTypeData TypeData { get; }

        public string Id { get; }

        public string Type => GetType().Name;

        public IList<string> InConnectors { get; }

        public IList<string> OutConnectors { get; }

        public string Name { get; set; }
    }
}
