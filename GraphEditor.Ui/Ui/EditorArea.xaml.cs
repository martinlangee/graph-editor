using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphEditor.ViewModel;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für EditorArea.xaml
    /// </summary>
    public partial class EditorArea : UserControl
    {
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
            ViewModel.DeselectAll();
        }
    }
}
