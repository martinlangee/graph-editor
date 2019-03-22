using GraphEditor.Tools;

namespace GraphEditor.ViewModel
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
                SetProperty<ConnectorStateViewModel, bool>(ref _isConnecting, value, nameof(IsConnecting),
                    (node, connecting) => MessageHub.Inst.NotifyConnectRequested(connecting, _node, Index));
            }
        }

        public bool IsConnectRequested
        {
            get { return _isConnectRequested; }
            set { SetProperty<ConnectorStateViewModel, bool>(ref _isConnectRequested, value, nameof(IsConnectRequested)); }
        }

        public bool IsOutBound { get; set; }
    }
}
