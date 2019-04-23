#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphEditor.Ui.ViewModel;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für ConnectorButton.xaml
    /// </summary>
    public partial class ConnectorButton : UserControl
    {
        ConnectorViewModel _viewModel;
        private ConnectorViewModel ViewModel => _viewModel = _viewModel ?? (ConnectorViewModel)DataContext;

        public ConnectorButton()
        {
            InitializeComponent();
        }

        private void Border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.IsConnecting = !ViewModel.IsConnecting;

            if (ViewModel.IsConnecting)
                Application.Current.MainWindow.Cursor = Cursors.UpArrow;
            else
                Application.Current.MainWindow.Cursor = Cursors.Arrow;

            e.Handled = true;
        }
    }
}
