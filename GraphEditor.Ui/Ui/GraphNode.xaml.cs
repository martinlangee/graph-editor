using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GraphEditor.ViewModel;

namespace GraphEditor
{
    /// <summary>
    /// Interaktionslogik für GraphNode.xaml
    /// </summary>
    public partial class GraphNode : UserControl
    {
        private GraphNodeViewModel ViewModel => (GraphNodeViewModel) DataContext;

        public GraphNode()
        {
            InitializeComponent();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
        
            Mouse.SetCursor(Cursors.SizeAll);
            e.Handled = true;
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var pointToScreen = Mouse.GetPosition(this);

                var data = new DataObject();

                if (!ViewModel.IsSelected)
                    ViewModel.Area.DeselectAll();

                data.SetData("Object", this);
                data.SetData("Point", pointToScreen);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                ViewModel.Area.DeselectAll();
            ViewModel.IsSelected = !ViewModel.IsSelected;

            e.Handled = true;
        }
    }
}