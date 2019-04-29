#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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
