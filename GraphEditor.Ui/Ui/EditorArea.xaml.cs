using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphEditor.ViewModel;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für EditorArea.xaml
    /// </summary>
    public partial class EditorArea : UserControl
    {
        Path _connLine;
        const double LineThickness = 1.3;
        const double LineThicknessHovered = 3;

        public EditorArea()
        {
            InitializeComponent();

            DataContext = new AreaViewModel(OnAddNode, OnRemoveNode, OnNodeLocationChanged, OnAddConnection, OnRemoveConnection);
        }

        private AreaViewModel ViewModel => (AreaViewModel) DataContext;

        private List<GraphNode> GraphNodes => _canvas.Children.OfType<GraphNode>().ToList();

        internal List<GraphNode> SelectedNodes => GraphNodes.Where(gn => gn.ViewModel.IsSelected).ToList();

        internal GraphNode NodeOfModel(NodeViewModel viewModel) => GraphNodes.FirstOrDefault(gn => gn.ViewModel.Equals(viewModel));

        private List<Polyline> ConnectionLines => _canvas.Children.OfType<Polyline>().ToList();

        private Polyline LineOfModel(ConnectionViewModel viewModel) => ConnectionLines.FirstOrDefault(cp => cp.DataContext.Equals(viewModel));

        private void OnAddNode(NodeViewModel nodeVm)
        {
            var graphNode = new GraphNode { DataContext = nodeVm };

            _canvas.Children.Add(graphNode);

            var mousePos = Mouse.GetPosition(_canvas);
            Canvas.SetLeft(graphNode, mousePos.X);
            Canvas.SetTop(graphNode, mousePos.Y);
        }

        private void OnRemoveNode(NodeViewModel nodeVm)
        {
            var toRemove = GraphNodes.FirstOrDefault(gn => gn.DataContext.Equals(nodeVm));
            _canvas.Children.Remove(toRemove);
        }

        private void OnNodeLocationChanged(NodeViewModel nodeVm, Point location)
        {
            var node = NodeOfModel(nodeVm);
            Canvas.SetLeft(node, location.X);
            Canvas.SetTop(node, location.Y);
        }

        private void OnAddConnection(ConnectionViewModel connectionVm)
        {
            var p1 = NodeOfModel(connectionVm.SourceNode).OutConnectorLocation(_canvas, connectionVm.SourceConnector);
            var p2 = NodeOfModel(connectionVm.TargetNode).InConnectorLocation(_canvas, connectionVm.TargetConnector);
            var line = new Polyline
            {
                Stroke = Brushes.DimGray,
                StrokeThickness = LineThickness,
                DataContext = connectionVm
            };

            line.Points.Add(p1);
            line.Points.Add(p2);

            line.MouseDown += Line_MouseDown;
            line.MouseMove += Line_MouseMove;
            line.MouseLeave += Line_MouseLeave;
            line.DataContext = connectionVm;
            _canvas.Children.Add(line);
        }

        private void OnUpdateConnections(NodeViewModel nodeVm)
        {
            var node = NodeOfModel(nodeVm);

            foreach (var line in ConnectionLines)
            {
                var connVm = (ConnectionViewModel) line.DataContext;
                if (nodeVm.Equals(connVm.SourceNode))
                {
                    line.Points[0] = node.OutConnectorLocation(_canvas, connVm.SourceConnector);
                }
                if (nodeVm.Equals(connVm.TargetNode))
                {
                    line.Points[line.Points.Count - 1] = node.InConnectorLocation(_canvas, connVm.TargetConnector);
                }
            }
        }

        private void OnRemoveConnection(ConnectionViewModel connectionVm)
        {
            // TODO
        }

        private void SetDragObjectPosition(DragEventArgs e)
        {
            // Position von allen Nodes setzen, die beim Draggen selektiert sind
            var nodeVMs = (List<NodeViewModel>) e.Data.GetData("Objects");
            var points = (List<Point>) e.Data.GetData("Points");

            for (var idx = 0; idx < nodeVMs.Count; idx++)
            {
                var point = e.GetPosition(_canvas) - points[idx];
                nodeVMs[idx].Location = new Point(point.X, point.Y);

                OnUpdateConnections(nodeVMs[idx]);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            SetDragObjectPosition(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            SetDragObjectPosition(e);
        }

        private void _canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_connLine != null)
            {
                
            }

            _connLine = null;

            ViewModel.DeselectAll();
        }

        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // TODO: Connection selktieren und entspr visualisieren; Deselektierung berücksichtigen
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            ((Polyline) sender).StrokeThickness = LineThicknessHovered;
        }

        private void Line_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Polyline) sender).StrokeThickness = LineThickness;
        }
    }
}
