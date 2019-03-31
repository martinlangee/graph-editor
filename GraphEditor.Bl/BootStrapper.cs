using GraphEditor.Bl.Persistence;
using GraphEditor.Interfaces.Container;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Persistence;
using GraphEditor.Nodes.Bl;
using GraphEditor.Ui.ViewModel;

namespace GraphEditor.Bl
{
    public class BootStrapper
    {
        public static void InitServices()
        {
            using (ServiceContainer.RegisterTransaction())
            {
                ServiceContainer.Register<NodeTypeRepository, INodeTypeRepository>();
                ServiceContainer.Register<AreaViewModel>();
                ServiceContainer.Register<GraphPersistence, IGraphPersistence>();
            }
        }

        public static void FinalizeServices()
        {
            ServiceContainer.FinalizeServices();
        }
    }
}
