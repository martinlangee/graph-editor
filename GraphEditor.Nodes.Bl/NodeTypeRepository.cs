using GraphEditor.Interfaces.Nodes;
using GraphEditor.Nodes.Bl.Nodes;
using System.Collections.Generic;

namespace GraphEditor.Nodes.Bl
{
    public class NodeTypeRepository: INodeTypeRepository
    {
        public NodeTypeRepository()
        {
            NodeTypes = new List<INodeTypeData>();

            NodeTypes.Add(new LogicalANDType());
        }

        public IList<INodeTypeData> NodeTypes { get; }
    }
}
