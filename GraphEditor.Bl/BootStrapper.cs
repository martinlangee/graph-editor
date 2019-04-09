using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Ui.ViewModel;
using GraphEditor.MyNodes.Types;

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
            }
        }

        public static void FinalizeServices()
        {
            ServiceContainer.FinalizeServices();
        }
    }
}
