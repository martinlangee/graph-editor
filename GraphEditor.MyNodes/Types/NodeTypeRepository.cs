using GraphEditor.Interface.Nodes;
using GraphEditor.MyNodes.LogicalAND;
using GraphEditor.MyNodes.LogicalXOR;
using GraphEditor.MyNodes.StartUpWarning;
using GraphEditor.MyNodes.LogicalOR;

namespace GraphEditor.MyNodes.Types
{
    public class NodeTypeRepository : NodeTypeRepositoryBase
    {
        protected override void CreateNodeTypes()
        {
            // adding some sample NodeTypes to the repository
            NodeTypes.Add(new LogicalANDType());
            NodeTypes.Add(new LogicalORType());
            NodeTypes.Add(new LogicalXORType());
            NodeTypes.Add(new StartUpWarningType());
        }
    }
}
