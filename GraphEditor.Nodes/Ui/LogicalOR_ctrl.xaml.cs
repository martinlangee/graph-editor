using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
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

namespace GraphEditor.Nodes.Ui
{
    /// <summary>
    /// Interaktionslogik für LogicalOR_ctrl.xaml
    /// </summary>
    public partial class LogicalOR_ctrl : UserControl, INodeConfigUi
    {
        private INodeData _nodeData;

        public LogicalOR_ctrl(INodeData nodeData)
        {
            InitializeComponent();

            _nodeData = nodeData;
        }

        public event Action<UserControl> OnClose;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke(this);
        }
    }
}
