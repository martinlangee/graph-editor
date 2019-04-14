using GraphEditor.Interface.Nodes;
using GraphEditor.Ui.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für EditorToolBar.xaml
    /// </summary>
    public partial class EditorToolBar : UserControl
    {
        public EditorToolBar()
        {
            InitializeComponent();
        }

        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject();

                var nodeType = ((IBaseNodeTypeData) ((FrameworkElement) sender).DataContext);

                data.SetData(UiConst.DragDropData_NodeType, nodeType);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }

        }
    }
}
