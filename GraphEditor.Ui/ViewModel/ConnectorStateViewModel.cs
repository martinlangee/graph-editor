using GraphEditor.Ui.Tools;

namespace GraphEditor.Ui.ViewModel
{
    public class ConnectorStateViewModel : BaseNotification
    {
        private NodeViewModel _node;
        private bool _isConnecting;
        private bool _isConnectRequested;

        public int Index { get; set; }

        public ConnectorStateViewModel(NodeViewModel node, int index, bool isOutBound)
        {
            _node = node;
            Index = index;
            IsOutBound = isOutBound;
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
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
                        SetProperty<ConnectorStateViewModel, bool>(ref _isConnecting, value, nameof(IsConnecting),
                            (n, isConnecting) => UiMessageHub.NotifyConnectRequested(isConnecting, _node, Index, IsOutBound));
                    }
                }
            }
        }

        public bool IsConnectRequested
        {
            get { return _isConnectRequested; }
            set
            {
                if (!IsConnected || !value)
                {
                    SetProperty<ConnectorStateViewModel, bool>(ref _isConnectRequested, value, nameof(IsConnectRequested));
                }
            }
        }

        public bool IsConnected { get; set; }

        public bool IsOutBound { get; set; }
    }
}
