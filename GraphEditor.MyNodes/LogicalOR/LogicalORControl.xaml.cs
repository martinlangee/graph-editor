using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
using System.Windows.Controls;

namespace GraphEditor.MyNodes.LogicalOR
{
    /// <summary>
    /// Interaktionslogik für LogicalORControl.xaml
    /// </summary>
    public partial class LogicalORControl : UserControl, INodeConfigUi
    {
        public LogicalORControl(INodeData nodeData)
        {
            InitializeComponent();

            DataContext = nodeData;
        }

        public string Title => (DataContext as INodeData).TypeData.Name;
    }
}
