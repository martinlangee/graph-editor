using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Bl.Nodes;
using System;
using System.Collections.Generic;

namespace GraphEditor.Nodes.Bl
{
    public class NodeTypeRepository: INodeTypeRepository
    {
        public NodeTypeRepository()
        {
            NodeTypes = new List<INodeTypeData>
            {
                new LogicalANDType(),
                new LogicalORType(),
                new LogicalXORType()
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
