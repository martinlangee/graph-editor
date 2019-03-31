using GraphEditor.Interfaces.Container;
using GraphEditor.Interfaces.Nodes;
using System.Collections.ObjectModel;

namespace GraphEditor.Ui.ViewModel
{
    public class ToolBarViewModel
    {
        public ToolBarViewModel()
        {
            NodeTypes = new ObservableCollection<INodeTypeData>(ServiceContainer.Get<INodeTypeRepository>().NodeTypes);
        }

        public ObservableCollection<INodeTypeData> NodeTypes { get; }
    }
}
