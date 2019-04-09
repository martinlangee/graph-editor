using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class ConnectionViewModel: BaseNotification
    {
        const double MaxBendPointDragHitDist = 100;
        const char PointsSeperator = ' ';

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

        private Point GetClosestPointOnLine(Point start, Point end, Point p)
        {
            double length = (start - end).LengthSquared;
            if (length.Equals(0.0)) return start;

            Vector vect = end - start;
            double param = (p - start) * vect / length;

            return (param < 0.0)
                ? start
                : (param > 1.0)
                    ? end
                    : (start + param * vect);
        }

        private Tuple<int, Point> GetClosestPointOnPolyline(Point p)
        {
            List<Tuple<int, Point, double>> closePoints = new List<Tuple<int, Point, double>>();
            Point current = _points[0];

            for (int pointIdx = 1; pointIdx < _points.Count; pointIdx++)
            {
                Point next = _points[pointIdx];
                Point closestPoint = GetClosestPointOnLine(current, next, p);
                double distSquared = (closestPoint - p).LengthSquared;
                closePoints.Add(new Tuple<int, Point, double>(pointIdx, closestPoint, distSquared));
                current = next;
            }
            
            var closePoint = closePoints.OrderBy(t => t.Item3).First();

            return new Tuple<int, Point>(closePoint.Item1, closePoint.Item2);
        }

        public int NearestBendPointIndex(Point point)
        {
            var nearest = _points.Select(p => new Tuple<Point, double>(p, (p - point).LengthSquared)).OrderBy(t => t.Item2).First();

            if (nearest.Item2 > MaxBendPointDragHitDist) return -1;

            return _points.IndexOf(nearest.Item1);
        }

        public void MovePoint(int index, Point point)
        {
            _points[index] = new Point(point.X, point.Y);
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

        public void InsertPointNear(Point point)
        {
            var closestPoint = GetClosestPointOnPolyline(point);
            InsertPoint(closestPoint.Item1, closestPoint.Item2);
        }

        public void RemovePointNear(Point point)
        {
            var closestPoint = GetClosestPointOnPolyline(point);
            RemovePointAt(closestPoint.Item1);
        }

        public void Remove()
        {
            SourceNode.RemoveConnection(this);
        }

        public void LoadFromToXml(XElement connectionXml)
        {
            var points = connectionXml.Attribute("Points").Value;
            var pointList = points.Split(PointsSeperator).Select(pt => pt.ToPoint());

            pointList.For((pt, i) => InsertPoint(i + 1, pt));
        }

        public void SaveToXml(XElement parentXml)
        {
            var connXml = new XElement("Connection");

            connXml.SetAttributeValue("Source", SourceNode.Data.Id);
            connXml.SetAttributeValue("SourceConn", SourceConnector);
            connXml.SetAttributeValue("Target", TargetNode.Data.Id);
            connXml.SetAttributeValue("TargetConn", TargetConnector);

            var pointsAttr = "";
            _points.For((pt, i) =>
            {
                // Start and end point not needed
                if (i == 0 || i == (_points.Count - 1)) return;

                pointsAttr += $"{pt}{PointsSeperator}";
            });

            if (!string.IsNullOrEmpty(pointsAttr))
                connXml.SetAttributeValue("Points", pointsAttr.Trim());

            parentXml.Add(connXml);
        }
    }
}
