using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Container;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Ui.Commands;
using System.Collections.ObjectModel;

namespace GraphEditor.Ui.ViewModel
{
    public class ToolBarViewModel: BaseNotification
    {
        bool _isGridShown;

        public ToolBarViewModel(RelayCommand loadCommand, RelayCommand saveCommand)
        {
            NodeTypes = new ObservableCollection<INodeTypeData>(ServiceContainer.Get<INodeTypeRepository>().NodeTypes);

            LoadCommand = loadCommand;
            SaveCommand = saveCommand;
        }

        public ObservableCollection<INodeTypeData> NodeTypes { get; }

        public bool IsGridShown
        {
            get { return _isGridShown; }
            set { SetProperty<NodeViewModel, bool>(ref _isGridShown, value, nameof(IsGridShown)); }
        }

        public RelayCommand LoadCommand { get; }

        public RelayCommand SaveCommand { get; }
    }
}
