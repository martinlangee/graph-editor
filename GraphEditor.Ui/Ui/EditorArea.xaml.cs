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
        Path _drawLine;

        public EditorArea()
        {
            InitializeComponent();

            DataContext = new EditorAreaViewModel(_canvas);
        }

        private EditorAreaViewModel ViewModel => (EditorAreaViewModel) DataContext;

        internal GraphNode NodeOfModel(GraphNodeViewModel viewModel) => GraphNodes.FirstOrDefault(gn => gn.ViewModel.Equals(viewModel));

        private List<GraphNode> GraphNodes => _canvas.Children.OfType<GraphNode>().ToList();

        internal List<GraphNode> SelectedNodes => GraphNodes.Where(gn => gn.ViewModel.IsSelected).ToList();

        private void SetDragObjectPosition(DragEventArgs e)
        {
            // Position von allen Nodes setzen, die beim Draggen selektiert sind
            var nodeVMs = (List<GraphNodeViewModel>) e.Data.GetData("Objects");
            var points = (List<Point>) e.Data.GetData("Points");

            for (var idx = 0; idx < nodeVMs.Count; idx++)
            {
                var point = e.GetPosition(_canvas) - points[idx];
                var node = NodeOfModel(nodeVMs[idx]);
                Canvas.SetLeft(node, point.X);
                Canvas.SetTop(node, point.Y);
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

        private void EditorArea_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ViewModel.SetCurrentMouse(new Point(e.CursorLeft, e.CursorTop));
        }

        private void _canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _drawLine = null;

            ViewModel.DeselectAll();
        }

        private void _canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_drawLine != null) return;

            _drawLine = new Path
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
            _drawLine.Stroke = Brushes.Blue;
            _drawLine.StrokeThickness = 1;
            _canvas.Children.Add(_drawLine);
        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawLine == null) return;
            
            ((LineSegment)((PathGeometry)_drawLine.Data).Figures[0].Segments[0]).Point = e.GetPosition(_canvas);
            
        }
    }
}
