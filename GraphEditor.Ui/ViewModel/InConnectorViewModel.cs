using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Utils;
using GraphEditor.Ui.Tools;
using System.Windows.Media;

namespace GraphEditor.Ui.ViewModel
{
    public class InConnectorViewModel : ConnectorViewModel
    {
        private InConnectorViewModel(NodeViewModel node, string name, int index) : base(node, name, index)
        {
            Brush = new SolidColorBrush(_nodeVm.Data.Ins[Index].Color.ToColor());
        }

        public static ConnectorViewModel Create(NodeViewModel nodeVm, string name, int index)
        {
            return new InConnectorViewModel(nodeVm, name, index);
        }

        protected override void NotifyConnectRequested(bool isConnecting, NodeViewModel nodeVm, int index)
        {
            UiMessageHub.NotifyConnectRequested(isConnecting, nodeVm, _nodeVm.Data.Ins[Index]);
        }

        public override byte[] Icon => _nodeVm.Data.Ins[Index].Icon;

        public override Brush Brush { get ; }

        public override bool IsOutBound => false;
    }
}
