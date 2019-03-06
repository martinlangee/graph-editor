using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class ConnectionViewModel: BaseNotification
    {
        NodeViewModel _sourceNode;
        NodeViewModel _targetNode;
        bool _isSelected;

        public ConnectionViewModel(NodeViewModel sourceNode, NodeViewModel targetNode, int sourceConn, int targetConn)
        {
            _sourceNode = sourceNode;
            _targetNode = targetNode;
            SourceConnector = sourceConn;
            TargetConnector = targetConn;

            Path = new ObservableCollection<Point>();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty<ConnectionViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }
        }

        public ObservableCollection<Point> Path { get; }

        public int SourceConnector { get; set; }
        public int TargetConnector { get; set; }

        public void Remove()
        {
            _sourceNode.RemoveConnection(this);
        }
    }
}
