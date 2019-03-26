using System.Collections.Generic;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class ConnectionViewModel: BaseNotification
    {
        bool _isSelected;
        List<Point> _points;

        public ConnectionViewModel(NodeViewModel sourceNode, NodeViewModel targetNode, int sourceConn, int targetConn)
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
            SourceConnector = sourceConn;
            TargetConnector = targetConn;

            _points = new List<Point>();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty<ConnectionViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }
        }

        public IReadOnlyList<Point> Points => _points;

        public int SourceConnector { get; set; }
        public int TargetConnector { get; set; }

        public NodeViewModel SourceNode { get; set; }
        public NodeViewModel TargetNode { get; set; }

        private void NotifyPointsChanged()
        {
            FirePropertyChanged(nameof(Points));
        }

        public void SetPoint(int index, Point point)
        {
            _points[index] = point;
            NotifyPointsChanged();
        }

        public void AddPoint(Point point)
        {
            _points.Add(point);
            NotifyPointsChanged();
        }

        public void InsertPoint(int index, Point point)
        {
            _points.Insert(index, point);
            NotifyPointsChanged();
        }

        public void RemovePoint(Point point)
        {
            _points.Remove(point);
            NotifyPointsChanged();
        }

        public void RemovePointAt(int index)
        {
            _points.RemoveAt(index);
            NotifyPointsChanged();
        }

        public void Remove()
        {
            SourceNode.RemoveConnection(this);
        }
    }
}
