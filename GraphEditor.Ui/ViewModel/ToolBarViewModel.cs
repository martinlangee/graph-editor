#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Ui;
using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System.Collections.ObjectModel;

namespace GraphEditor.Ui.ViewModel
{
    public class ToolBarViewModel: BaseNotification, IToolBarViewModel
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
