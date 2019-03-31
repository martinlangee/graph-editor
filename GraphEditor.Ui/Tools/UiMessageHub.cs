using GraphEditor.Ui.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GraphEditor.Ui.Tools
{
    public class UiMessageHub
    {
        private static Timer _updateTimer = new Timer(UpdateLocation, null, 500, 10);
        private static Dictionary<NodeViewModel, Point> _actNodePos = new Dictionary<NodeViewModel, Point>();

        private static void UpdateLocation(object state)
        {
            if (_actNodePos == null) return;

            Dispatcher?.Invoke(() =>
                {
                    foreach (var item in _actNodePos)
                    {
                        OnNodeLocationChanged?.Invoke(item.Key, item.Value);
                    }
                });
        }

        public static Dispatcher Dispatcher { get; set; }

        public static void AddNode(NodeViewModel node)
        {
            OnAddNode?.Invoke(node);
        }

        public static void RemoveNode(NodeViewModel node)
        {
            OnRemoveNode?.Invoke(node);
        }

        public static void NodeLocationChanged(NodeViewModel node, Point location)
        {
            if (!_actNodePos.ContainsKey(node))
                _actNodePos.Add(node, location);

            _actNodePos[node] = location;
        }

        public static void AddConnection(ConnectionViewModel connection)
        {
            OnAddConnection?.Invoke(connection);
        }

        public static void RemoveConnection(ConnectionViewModel connection)
        {
            OnRemoveConnection?.Invoke(connection);
        }

        public static void NotifyConnectRequested(bool isConnecting, NodeViewModel sourceNode, int connIdx, bool isOutBound)
        {
            OnConnectRequested?.Invoke(isConnecting, sourceNode, connIdx, isOutBound);
        }

        public static void CreateConnection(NodeViewModel targetNode, int connIdx)
        {
            OnCreateConnection?.Invoke(targetNode, connIdx);
        }

        public static void Dispose()
        {
            _updateTimer.Dispose();
            Thread.Sleep(100);
            _actNodePos = null;
            Dispatcher = null;
        }

        public static event Action<NodeViewModel> OnAddNode;
        public static event Action<NodeViewModel> OnRemoveNode;
        public static event Action<NodeViewModel, Point> OnNodeLocationChanged;
        public static event Action<ConnectionViewModel> OnAddConnection;
        public static event Action<ConnectionViewModel> OnRemoveConnection;
        public static event Action<bool, NodeViewModel, int, bool> OnConnectRequested;
        public static event Action<NodeViewModel, int> OnCreateConnection;
    }
}
