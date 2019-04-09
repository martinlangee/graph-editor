using GraphEditor.Interface.Container;
using GraphEditor.Ui.ViewModel;
using System.Windows;

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

            toolBar.DataContext = ServiceContainer.Get<AreaViewModel>().ToolBar;
        }
    }
}
