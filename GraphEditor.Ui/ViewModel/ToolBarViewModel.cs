using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System.Collections.ObjectModel;

namespace GraphEditor.Ui.ViewModel
{
    public class ToolBarViewModel: BaseNotification
    {
        bool _isGridVisible = true;
        bool _isEnabled = true;
        bool _showLabels = true;

        public ToolBarViewModel(RelayCommand loadCommand, RelayCommand saveCommand, RelayCommand switchStatesCommand, RelayCommand resetStatesCommand)
        {
            NodeTypes = new ObservableCollection<INodeTypeData>(ServiceContainer.Get<INodeTypeRepository>().NodeTypes);

            LoadCommand = loadCommand;
            SaveCommand = saveCommand;
            SwitchStatesCommand = switchStatesCommand;
            ResetStatesCommand = resetStatesCommand;

            ShowLabels = UiStates.ShowLabels;
        }

        public ObservableCollection<INodeTypeData> NodeTypes { get; }

        public bool IsGridVisible { get => _isGridVisible; set => SetProperty<ToolBarViewModel, bool>(ref _isGridVisible, value, nameof(IsGridVisible)); }

        public bool IsEnabled { get => _isEnabled; set => SetProperty<ToolBarViewModel, bool>(ref _isEnabled, value, nameof(IsEnabled)); }

        public bool ShowLabels { get => _showLabels; set => SetProperty<ToolBarViewModel, bool>(ref _showLabels, value, nameof(ShowLabels), (vm, val) => UiStates.ShowLabels = val); }

        public RelayCommand LoadCommand { get; }

        public RelayCommand SaveCommand { get; }

        public RelayCommand SwitchStatesCommand { get; }

        public RelayCommand ResetStatesCommand { get; }
    }
}
