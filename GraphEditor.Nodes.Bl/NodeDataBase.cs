using GraphEditor.Interfaces.Nodes;
using System;
using System.Collections.Generic;

namespace GraphEditor.Nodes.Bl
{
    public abstract class NodeDataBase : INodeData
    {
        public NodeDataBase()
        {
            Id = Guid.NewGuid().ToString();

            InConnectors = new List<string>();
            OutConnectors = new List<string>();
        }

        public string Id { get; }

        public string Type => GetType().Name;

        public IList<string> InConnectors { get; }

        public IList<string> OutConnectors { get; }
    }
}
