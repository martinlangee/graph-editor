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

        private Timer _updateTimer;
        private Dictionary<NodeViewModel, Point> _actNodePos = new Dictionary<NodeViewModel, Point>();

        public MessageHub()
        {
            _updateTimer = new Timer(UpdateLocation, null, 500, 10);
        }

        private void UpdateLocation(object state)
        {
            if (_actNodePos == null) return;

            Dispatcher?.Invoke(() =>
                {
                    foreach (var item in _actNodePos)
                    {
                        OnNodeLocationChanged?.Invoke(item.Key, item.Value);
                        OnUpdateConnections?.Invoke(item.Key);
                    }
                });
        }

        public Dispatcher Dispatcher { get; set; }

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

        public void AddConnection(ConnectionViewModel connection)
        {
            OnAddConnection?.Invoke(connection);
        }

        public void RemoveConnection(ConnectionViewModel connection)
        {
            OnRemoveConnection?.Invoke(connection);
        }

        public void NotifyConnectRequested(bool value, NodeViewModel sourceNode, int connectorIdx)
        {
            OnConnectRequested?.Invoke(value, sourceNode, connectorIdx);
        }

        public void Dispose()
        {
            _updateTimer.Dispose();
            Thread.Sleep(100);
            _actNodePos = null;
            Dispatcher = null;
        }

        public event Action<NodeViewModel> OnAddNode;
        public event Action<NodeViewModel> OnRemoveNode;
        public event Action<NodeViewModel, Point> OnNodeLocationChanged;
        public event Action<NodeViewModel> OnUpdateConnections;
        public event Action<ConnectionViewModel> OnAddConnection;
        public event Action<ConnectionViewModel> OnRemoveConnection;
        public event Action<bool, NodeViewModel, int> OnConnectRequested;
    }
}
