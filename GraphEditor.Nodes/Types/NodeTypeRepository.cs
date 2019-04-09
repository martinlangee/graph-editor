using GraphEditor.Interface.Nodes;
using GraphEditor.Nodes.LogicalAND;
using GraphEditor.Nodes.LogicalOR;
using GraphEditor.Nodes.LogicalXOR;
using GraphEditor.Nodes.StartUpWarning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor.Nodes.Types
{
    public class NodeTypeRepository : INodeTypeRepository
    {
        public NodeTypeRepository()
        {
            NodeTypes = new List<INodeTypeData>
            {
                new LogicalANDType(),
                new LogicalORType(),
                new LogicalXORType(),
                new StartUpWarningType()
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
