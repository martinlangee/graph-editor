#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
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