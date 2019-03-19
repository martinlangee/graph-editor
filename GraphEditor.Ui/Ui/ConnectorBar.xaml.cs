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
using GraphEditor.Tools;
using GraphEditor.Ui;
using GraphEditor.ViewModel;

namespace GraphEditor
{
    /// <summary>
    /// Interaktionslogik für ConnectorBar.xaml
    /// </summary>
    public partial class ConnectorBar : UserControl
    {
        public ConnectorBar()
        {
            InitializeComponent();
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var conn = (ConnectorViewModel)((FrameworkElement)sender).DataContext;
            conn.IsConnecting = !conn.IsConnecting;
        }
    }
}
