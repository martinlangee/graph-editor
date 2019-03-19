namespace GraphEditor.ViewModel
{
    public class ConnectorViewModel : BaseNotification
    {
        private NodeViewModel _node;
        private bool _isConnecting;

        public int Index { get; set; }

        public ConnectorViewModel(NodeViewModel node, int index, bool isOut)
        {
            _node = node;
            Index = index;
            IsOut = isOut;
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set { SetProperty<NodeViewModel, bool>(ref _isConnecting, value, nameof(IsConnecting)); }
        }

        public ConnectionViewModel Connection { get; set; }

        public bool IsOut { get; set; }
    }
}
