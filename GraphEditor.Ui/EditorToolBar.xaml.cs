#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

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

                var nodeType = ((INodeTypeData) ((FrameworkElement) sender).DataContext);

                data.SetData(UiConst.DragDropData_NodeType, nodeType);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }

        }
    }
}
