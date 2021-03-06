#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

using GraphEditor.Interface.Container;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Ui;
using GraphEditor.Interface.Utils;
using GraphEditor.Ui.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class ConnectionViewModel: BaseNotification
    {
        const double MaxBendPointDragHitDist = 100;
        const char PointsSeperator = ' ';

        private IXmlClasses _xmlClasses = ServiceContainer.Get<IXmlClasses>();
        bool _isSelected;
        readonly List<Point> _points;
        private object _state = Brushes.Gray;  // example using the state for color visualization of connections

        public ConnectionViewModel(NodeViewModel sourceNode, NodeViewModel targetNode, int sourceConn, int targetConn)
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
            SourceConnector = sourceConn;
            TargetConnector = targetConn;

            _points = new List<Point>();
        }

        public bool IsSelected { get => _isSelected; set => SetProperty<ConnectionViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }

        public object State { get { return _state; } set { SetProperty<ConnectorViewModel, object>(ref _state, value, nameof(State)); } }

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

        public int LastPointIndex => _points.Count - 1;

        public void MovePoint(int index, Point point)
        {
            if (point.Round().IsEqual(_points[index].Round())) return;

            _points[index] = point.Round();
            NotifyPointsChanged();
        }

        public void MovePoint(int index, double newY)
        {
            if (Math.Round(newY).Equals(Math.Round(_points[index].Y))) return;

            MovePoint(index, new Point(_points[index].X, newY));
        }

        public void MovePoint(int index, bool down)
        {
            MovePoint(index, new Point(_points[index].X, _points[index].Y + (down ? 2 : -2) * UiConst.GridWidth));
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
            var points = connectionXml.Attribute(_xmlClasses.Points)?.Value;
            var pointList = points?.Split(PointsSeperator).Select(pt => pt.ToPoint());

            pointList?.For((pt, i) => InsertPoint(i + 1, pt));
        }

        public void SaveToXml(XElement parentXml)
        {
            var connXml = new XElement(_xmlClasses.Connection);

            connXml.SetAttributeValue(_xmlClasses.Source, SourceNode.Data.Id);
            connXml.SetAttributeValue(_xmlClasses.SourceConn, SourceConnector);
            connXml.SetAttributeValue(_xmlClasses.Target, TargetNode.Data.Id);
            connXml.SetAttributeValue(_xmlClasses.TargetConn, TargetConnector);

            var pointsAttr = "";
            _points.For((pt, i) =>
            {
                pointsAttr += $"{pt}{PointsSeperator}";
            }, 1, _points.Count - 2);  // Start and end point not needed

            if (!string.IsNullOrEmpty(pointsAttr))
                connXml.SetAttributeValue(_xmlClasses.Points, pointsAttr.Trim());

            parentXml.Add(connXml);
        }
    }
}
