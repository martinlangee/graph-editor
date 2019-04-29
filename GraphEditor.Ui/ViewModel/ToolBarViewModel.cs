#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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
