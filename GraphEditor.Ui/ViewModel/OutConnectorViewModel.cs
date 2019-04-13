using GraphEditor.Interface.Utils;
using GraphEditor.Ui.Tools;
using System;
using System.Windows.Media;

namespace GraphEditor.Ui.ViewModel
{
    public class OutConnectorViewModel : ConnectorViewModel
    {
        private OutConnectorViewModel(NodeViewModel nodeVm, string name, int index) : base(nodeVm, name, index)
        {
            Brush = new SolidColorBrush(_nodeVm.Data.Outs[Index].Color.ToColor());
        }

        public static ConnectorViewModel Create(NodeViewModel nodeVm, string name, int index)
        {
            return new OutConnectorViewModel(nodeVm, name, index);
        }

        protected override void NotifyConnectRequested(bool isConnecting, NodeViewModel nodeVm, int index)
        {
            UiMessageHub.NotifyConnectRequested(isConnecting, nodeVm, index, isOutBound: true);
        }

        public override byte[] Icon => _nodeVm.Data.Outs[Index].Icon;

        public override Brush Brush { get ; }

        public override bool IsOutBound => true;
    }
}
