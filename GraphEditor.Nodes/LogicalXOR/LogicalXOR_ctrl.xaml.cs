using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.Nodes.LogicalXOR
{
    /// <summary>
    /// Interaktionslogik für LogicalXOR_ctrl.xaml
    /// </summary>
    public partial class LogicalXOR_ctrl : UserControl, INodeConfigUi
    {
        public LogicalXOR_ctrl(INodeData nodeData)
        {
            InitializeComponent();

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
