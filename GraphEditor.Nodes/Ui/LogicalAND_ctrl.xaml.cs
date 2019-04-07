using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.Nodes.Ui
{
    /// <summary>
    /// Interaktionslogik für LogicalAND_ctrl.xaml
    /// </summary>
    public partial class LogicalAND_ctrl : UserControl, INodeConfigUi
    {
        INodeData _nodeData;

        public LogicalAND_ctrl(INodeData nodeData)
        {
            InitializeComponent();

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
            _nodeData = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var a = ItemsControl.GetAlternationIndex(((CheckBox) sender).TemplatedParent);
        }
    }
}
