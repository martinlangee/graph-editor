using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
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

            DataContext = nodeData;
        }

        public string Title => (DataContext as INodeData).TypeData.Name;
    }
}
