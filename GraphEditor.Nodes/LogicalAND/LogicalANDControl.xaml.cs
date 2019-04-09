using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.MyNodes.LogicalAND
{
    /// <summary>
    /// Interaktionslogik für LogicalANDControl.xaml
    /// </summary>
    public partial class LogicalANDControl : UserControl, INodeConfigUi
    {
        public LogicalANDControl(INodeData nodeData)
        {
            InitializeComponent();

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
