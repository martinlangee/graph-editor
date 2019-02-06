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

namespace GraphEditor
{
    /// <summary>
    /// Interaktionslogik für GraphNode.xaml
    /// </summary>
    public partial class GraphNode : UserControl
    {
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

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                var pointToScreen = Mouse.GetPosition(this);

                var data = new DataObject();
                data.SetData("Object", this);
                data.SetData("Point", pointToScreen);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }
    }
}
