namespace GraphEditor.ViewModel
{
    public class ConnectorStateViewModel : BaseNotification
    {
        private NodeViewModel _node;
        private bool _isConnecting;

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
            set { SetProperty<NodeViewModel, bool>(ref _isConnecting, value, nameof(IsConnecting)); }
        }
        public bool IsOutBound { get; set; }
    }
}
