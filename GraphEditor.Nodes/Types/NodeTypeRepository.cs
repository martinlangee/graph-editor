using GraphEditor.Interface.Nodes;
using GraphEditor.MyNodes.LogicalAND;
using GraphEditor.MyNodes.LogicalXOR;
using GraphEditor.MyNodes.StartUpWarning;
using GraphEditor.MyNodes.LogicalOR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor.MyNodes.Types
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
