using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Types;
using System;
using System.Collections.Generic;

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

        // called by IoC container
        public void OnBuiltUp()
        {
            Console.Write("NodeTypeRepository is built up");
        }

        // called by IoC container
        public void ShutDown()
        {
            Console.Write("NodeTypeRepository is shut down");
        }
    }
}
