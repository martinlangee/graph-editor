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

            DataContext = new EditorAreaViewModel();
        }

        private void Canvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            var contextMenu = canvas.ContextMenu;

            if (contextMenu == null) return;

            contextMenu.PlacementTarget = canvas;
            contextMenu.IsOpen = true;
        }
    }
}
