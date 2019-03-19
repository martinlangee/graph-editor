using GraphEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GraphEditor.Tools
{
    public class MessageHub
    {
        static MessageHub _instance;
        public static MessageHub Inst => _instance = _instance ?? new MessageHub();

        public void AddNode(NodeViewModel node)
        {
            OnAddNode?.Invoke(node);
        }

        public void RemoveNode(NodeViewModel node)
        {
            OnRemoveNode?.Invoke(node);
        }

        public void NodeLocationChanged(NodeViewModel node, Point location)
        {
            OnNodeLocationChanged?.Invoke(node, location);
        }

        public void UpdateConnections(NodeViewModel node)
        {
            OnUpdateConnections?.Invoke(node);
        }

        public void AddConnection(ConnectionViewModel connection)
        {
            OnAddConnection?.Invoke(connection);
        }

        public void RemoveConnection(ConnectionViewModel connection)
        {
            OnRemoveConnection?.Invoke(connection);
        }

        public event Action<NodeViewModel> OnAddNode;
        public event Action<NodeViewModel> OnRemoveNode;
        public event Action<NodeViewModel, Point> OnNodeLocationChanged;
        public event Action<NodeViewModel> OnUpdateConnections;
        public event Action<ConnectionViewModel> OnAddConnection;
        public event Action<ConnectionViewModel> OnRemoveConnection;
    }
}
