using GraphEditor.Interface.Container;
using GraphEditor.Interface.Ui;
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

            toolBar.DataContext = ServiceContainer.Get<IAreaViewModel>().ToolBar;
        }
    }
}
