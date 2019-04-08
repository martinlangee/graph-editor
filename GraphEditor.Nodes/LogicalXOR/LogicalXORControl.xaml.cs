using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.Nodes.LogicalXOR
{
    /// <summary>
    /// Interaktionslogik für LogicalXORControl.xaml
    /// </summary>
    public partial class LogicalXORControl : UserControl, INodeConfigUi
    {
        public LogicalXORControl(INodeData nodeData)
        {
            InitializeComponent();

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
