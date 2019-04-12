using GraphEditor.Interface.ConfigUi;
using GraphEditor.Ui.Tools;
using System.Windows.Media;

namespace GraphEditor.Ui.ViewModel
{
    public class ConnectorViewModel : BaseNotification
    {
        private readonly NodeViewModel _node;
        private bool _isConnecting;
        private bool _isConnectRequested;
        private bool _isActive = true;
        private bool _showLabels = true;

        public int Index { get; set; }

        public ConnectorViewModel(NodeViewModel node, string name, int index, bool isOutBound)
        {
            _node = node;
            Name = name;
            Index = index;
            IsOutBound = isOutBound;

            UiStates.OnShowLabelsChanged += OnShowLabelsChanged;
        }

        private void OnShowLabelsChanged(bool visible)
        {
            ShowLabels = visible;
        }

        public bool IsConnecting
        {
            get => _isConnecting; 
            set
            {
                if (value && IsConnectRequested)
                {
                    UiMessageHub.CreateConnection(_node, Index);
                }
                else
                {
                    if (!IsConnected || !value)
                    {
                        SetProperty<ConnectorViewModel, bool>(ref _isConnecting, value, nameof(IsConnecting),
                            (n, isConnecting) => UiMessageHub.NotifyConnectRequested(isConnecting, _node, Index, IsOutBound));
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

        public bool IsOutBound { get; set; }

        public string Name { get; }

        public byte[] Icon => IsOutBound ? _node.Data.Outs[Index].Icon : _node.Data.Ins[Index].Icon;

        public object Type => IsOutBound 
            ? new SolidColorBrush(_node.Data.Outs[Index].TypeAsColor) 
            : new SolidColorBrush(_node.Data.Ins[Index].TypeAsColor);
    }
}