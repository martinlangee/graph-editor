using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.Nodes.LogicalOR
{
    /// <summary>
    /// Interaktionslogik für LogicalORControl.xaml
    /// </summary>
    public partial class LogicalORControl : UserControl, INodeConfigUi
    {
        public LogicalORControl(INodeData nodeData)
        {
            InitializeComponent();

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
