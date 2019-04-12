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
