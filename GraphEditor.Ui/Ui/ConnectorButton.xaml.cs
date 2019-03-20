using System.Windows.Controls;
using System.Windows.Input;
using GraphEditor.Tools;
using GraphEditor.ViewModel;

namespace GraphEditor
{
    /// <summary>
    /// Interaktionslogik für ConnectorButton.xaml
    /// </summary>
    public partial class ConnectorButton : UserControl
    {
        ConnectorStateViewModel _viewModel;
        private ConnectorStateViewModel ViewModel => _viewModel = _viewModel ?? (ConnectorStateViewModel)DataContext;

        public ConnectorButton()
        {
            InitializeComponent();
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.IsConnecting = !ViewModel.IsConnecting;
        }
    }
}
