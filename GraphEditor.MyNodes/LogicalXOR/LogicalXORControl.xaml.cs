using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.MyNodes.LogicalXOR
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
