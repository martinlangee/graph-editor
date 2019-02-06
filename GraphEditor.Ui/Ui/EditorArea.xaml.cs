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

        private void SetDragObjectPosition(DragEventArgs e)
        {
            var gn = (GraphNode) e.Data.GetData("Object");
            var pos = (Point) e.Data.GetData("Point");

            var p = e.GetPosition(_canvas) - pos;

            Canvas.SetLeft(gn, p.X);
            Canvas.SetTop(gn, p.Y);
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
    }
}
