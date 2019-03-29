using GraphEditor.Bl.Persistence;
using GraphEditor.Interfaces.Container;
using GraphEditor.Interfaces.Persistence;
using GraphEditor.ViewModel;

namespace GraphEditor.Bl
{
    public class BootStrapper
    {
        public static void InitServices()
        {
            ServiceContainer.Register<AreaViewModel>();
            ServiceContainer.Register<GraphPersistence, IGraphPersistence>();
        }
    }
}
