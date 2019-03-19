using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.ViewModel
{
    public class ConnectorViewModel : BaseNotification
    {
        private NodeViewModel _node;
        private bool _isConnecting;

        public int Index { get; set; }

        public ConnectorViewModel(NodeViewModel node, int index)
        {
            _node = node;
            Index = index;
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set { SetProperty<NodeViewModel, bool>(ref _isConnecting, value, nameof(IsConnecting)); }
        }

        public ConnectionViewModel Connection { get; set; }
    }
}
