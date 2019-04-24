#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using GraphEditor.Interface.Container;
using GraphEditor.Interface.Ui;
using System.Windows;
using System.Windows.Input;

namespace DebugApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            toolBar.DataContext = ServiceContainer.Get<IAreaViewModel>().ToolBar;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            editorArea.HandleKeyDown(sender, e);
        }
    }
}
