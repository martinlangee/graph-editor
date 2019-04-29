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

using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Ui;
using GraphEditor.Ui.Tools;
using System.Windows.Media;

namespace GraphEditor.Ui.ViewModel
{
    public abstract class ConnectorViewModel : BaseNotification
    {
        protected readonly NodeViewModel _nodeVm;
        private bool _isConnecting;
        private bool _isConnectRequested;
        private bool _isActive = true;
        private bool _showLabels = true;

        public int Index { get; set; }

        protected ConnectorViewModel(NodeViewModel node, string name, int index)
        {
            _nodeVm = node;
            Name = name;
            Index = index;

            UiStates.OnShowLabelsChanged += OnShowLabelsChanged;
        }

        public static ConnectorViewModel Create(NodeViewModel nodeVm, string name, int index, bool isOutBound)
        {
            var connVm = isOutBound ? OutConnectorViewModel.Create(nodeVm, name, index) : InConnectorViewModel.Create(nodeVm, name, index);
            connVm.ShowLabels = UiStates.ShowLabels;
            return connVm;
        }

        private void OnShowLabelsChanged(bool visible)
        {
            ShowLabels = visible;
        }

        protected abstract void NotifyConnectRequested(bool isConnecting, NodeViewModel sourceNodeVm, int index);

        public bool IsConnecting
        {
            get => _isConnecting; 
            set
            {
                if (value && IsConnectRequested)
                {
                    UiMessageHub.CreateConnection(_nodeVm, Index);
                }
                else
                {
                    if (!IsConnected || !value)
                    {
                        SetProperty<ConnectorViewModel, bool>(ref _isConnecting, value, nameof(IsConnecting),
                            (n, isConnecting) => NotifyConnectRequested(isConnecting, _nodeVm, Index));
                    }
                }
            }
        }

        public bool IsConnectRequested
        {
            get => _isConnectRequested;
            set
            {
                if (!IsConnected || !value)
                {
                    SetProperty<ConnectorViewModel, bool>(ref _isConnectRequested, value, nameof(IsConnectRequested));
                }
            }
        }

        public bool IsActive { get => _isActive; set => SetProperty<ConnectorViewModel, bool>(ref _isActive, value, nameof(IsActive)); }

        public bool ShowLabels { get => _showLabels; set => SetProperty<ConnectorViewModel, bool>(ref _showLabels, value, nameof(ShowLabels)); }
        
        public bool IsConnected { get; set; }

        public abstract bool IsOutBound { get; }

        public string Name { get; }

        public abstract byte[] Icon { get; }

        public abstract Brush Brush { get; }
    }
}