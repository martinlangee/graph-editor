using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor.Nodes
{
    public class NodeTypeRepository: INodeTypeRepository
    {
        public NodeTypeRepository()
        {
            NodeTypes = new List<INodeTypeData>
            {
                new LogicalAND_Type(),
                new LogicalOR_Type(),
                new LogicalXOR_Type()
            };
        }

        public IList<INodeTypeData> NodeTypes { get; }

        public INodeTypeData Find(string type)
        {
            return NodeTypes.FirstOrDefault(nt => nt.Type.Equals(type));
        }

        // called by IoC container
        public void OnBuiltUp()
        {
            Console.Write("NodeTypeRepository is built up");
        }

        // called by IoC container
        public void TearDown()
        {
            Console.Write("NodeTypeRepository is shut down");
        }
    }
}
