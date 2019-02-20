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
using GraphEditor.Ui;
using GraphEditor.ViewModel;

namespace GraphEditor
{
    /// <summary>
    /// Interaktionslogik für GraphNode.xaml
    /// </summary>
    public partial class GraphNode : UserControl
    {
        internal NodeViewModel ViewModel => (NodeViewModel) DataContext;

        public GraphNode()
        {
            InitializeComponent();
        }

        EditorAreaViewModel AreaVm => ViewModel.Area;

        EditorArea Area => (EditorArea) ((FrameworkElement) Parent).Parent;

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
                var data = new DataObject();

                if (!ViewModel.IsSelected)
                    AreaVm.DeselectAll();

                ViewModel.IsSelected = true;

                var pointsToScreen = AreaVm.Selected.Select(nodeVm => Mouse.GetPosition(Area.NodeOfModel(nodeVm))).ToList();

                data.SetData("Objects", AreaVm.Selected);
                data.SetData("Points", pointsToScreen);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
               AreaVm.DeselectAll();
            ViewModel.IsSelected = !ViewModel.IsSelected;

            e.Handled = true;
        }

        private void OutConnector_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseButtonEventArgs newarg = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice)
            {
                RoutedEvent = MouseLeftButtonDownEvent,
                Source = sender
            };
            Area._canvas.RaiseEvent(newarg);            
        }
    }
}