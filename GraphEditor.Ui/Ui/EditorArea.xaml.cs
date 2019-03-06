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

        internal GraphNode NodeOfModel(NodeViewModel viewModel) => GraphNodes.FirstOrDefault(gn => gn.ViewModel.Equals(viewModel));

        private List<GraphNode> GraphNodes => _canvas.Children.OfType<GraphNode>().ToList();

        internal List<GraphNode> SelectedNodes => GraphNodes.Where(gn => gn.ViewModel.IsSelected).ToList();

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

        private void OnAddConnection(ConnectionViewModel connVm)
        {
            // TODO
        }

        private void OnRemoveConnection(ConnectionViewModel connVm)
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

        private void _canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_connLine != null) return;
            if (sender is Border) return;
            if (!Equals(((FrameworkElement) e.OriginalSource).Tag, "OutConnector")) return;

            _connLine = new Path
            {
                Data = new PathGeometry
                (
                    new List<PathFigure>
                    {
                        new PathFigure
                        (
                            e.GetPosition(_canvas),
                            new List<LineSegment>{new LineSegment(e.GetPosition(_canvas), true)},
                            false
                        )
                    }
                )
            };
            _connLine.Stroke = Brushes.DimGray;
            _connLine.StrokeThickness = LineThickness;
            _connLine.MouseDown += Line_MouseDown;
            _connLine.MouseMove += Line_MouseMove;
            _connLine.MouseLeave += Line_MouseLeave;
            _canvas.Children.Add(_connLine);
         }

        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // TODO: Connection selktieren und entspr visualisieren; Deselektierung berücksichtigen
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            ((Path) sender).StrokeThickness = LineThicknessHovered;
        }

        private void Line_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Path) sender).StrokeThickness = LineThickness;
        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_connLine == null) return;
            
            ((LineSegment) ((PathGeometry) _connLine.Data).Figures[0].Segments[0]).Point = e.GetPosition(_canvas);

            e.Handled = true;            
        }
    }
}
