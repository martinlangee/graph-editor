using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class ConnectionViewModel: BaseNotification
    {
        bool _isSelected;

        public ConnectionViewModel(NodeViewModel sourceNode, NodeViewModel targetNode, int sourceConn, int targetConn)
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
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

        public NodeViewModel SourceNode { get; set; }
        public NodeViewModel TargetNode { get; set; }

        public void Remove()
        {
            SourceNode.RemoveConnection(this);
        }
    }
}
