using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.Nodes.LogicalOR
{
    /// <summary>
    /// Interaktionslogik für LogicalOR_ctrl.xaml
    /// </summary>
    public partial class LogicalOR_ctrl : UserControl, INodeConfigUi
    {
        public LogicalOR_ctrl(INodeData nodeData)
        {
            InitializeComponent();

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
