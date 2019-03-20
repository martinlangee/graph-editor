using GraphEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GraphEditor.Tools
{
    public class MessageHub: IDisposable
    {
        static MessageHub _instance;
        public static MessageHub Inst => _instance = _instance ?? new MessageHub();

        Timer _updateTimer;
        Dictionary<NodeViewModel, Point> _actNodePos = new Dictionary<NodeViewModel, Point>();
        Dispatcher _dispatcher = Application.Current.Dispatcher;

        public MessageHub()
        {
            _updateTimer = new Timer(DoUpdateLocation, null, 500, 10);
        }

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
            if (!_actNodePos.ContainsKey(node))
                _actNodePos.Add(node, location);

            _actNodePos[node] = location;
        }

        private void DoUpdateLocation(object state)
        {
            if (_actNodePos == null) return;
            if (Application.Current == null) return;

            _dispatcher.Invoke(() =>
            {
                foreach (var item in _actNodePos)
                {
                    OnNodeLocationChanged?.Invoke(item.Key, item.Value);
                    OnUpdateConnections?.Invoke(item.Key);
                }
            });
        }

        public void AddConnection(ConnectionViewModel connection)
        {
            OnAddConnection?.Invoke(connection);
        }

        public void RemoveConnection(ConnectionViewModel connection)
        {
            OnRemoveConnection?.Invoke(connection);
        }

        public void Dispose()
        {
            _actNodePos = null;
        }

        public event Action<NodeViewModel> OnAddNode;
        public event Action<NodeViewModel> OnRemoveNode;
        public event Action<NodeViewModel, Point> OnNodeLocationChanged;
        public event Action<NodeViewModel> OnUpdateConnections;
        public event Action<ConnectionViewModel> OnAddConnection;
        public event Action<ConnectionViewModel> OnRemoveConnection;
    }
}
