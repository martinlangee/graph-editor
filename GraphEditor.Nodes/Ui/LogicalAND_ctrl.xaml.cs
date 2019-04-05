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

            Header.OnClose += () => OnClose?.Invoke(this);

            _nodeData = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
