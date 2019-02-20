using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class ConnectionViewModel: BaseNotification
    {
        GraphNodeViewModel _sourceNode;
        bool _isSelected;

        public ConnectionViewModel(GraphNodeViewModel sourceNode, int sourceConn)
        {
            _sourceNode = sourceNode;
            SourceConnector = sourceConn;

            Path = new ObservableCollection<Point>();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value, nameof(IsSelected)); }
        }

        public ObservableCollection<Point> Path { get; }

        public int SourceConnector { get; set; }
        public int TargetConnector { get; set; }
    }
}
