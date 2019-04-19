using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Ui.ViewModel;
using GraphEditor.MyNodes.Types;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Ui;

namespace GraphEditor.Bl
{
    public class BootStrapper
    {
        public static void InitServices()
        {
            using (ServiceContainer.RegisterTransaction())
            {
                ServiceContainer.Register<BaseXmlClasses, IXmlClasses>();
                ServiceContainer.Register<NodeTypeRepository, INodeTypeRepository>();
                ServiceContainer.Register<AreaViewModel, IAreaViewModel>();
            }
        }

        public static void FinalizeServices()
        {
            ServiceContainer.FinalizeServices();
        }
    }
}
