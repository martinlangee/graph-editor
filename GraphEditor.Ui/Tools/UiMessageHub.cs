#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using GraphEditor.Interface.Nodes;
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
        private static readonly Timer _updateTimer = new Timer(UpdateLocation, null, 500, 10);
        private static Dictionary<NodeViewModel, Point> _actNodePos = new Dictionary<NodeViewModel, Point>();

        private static void UpdateLocation(object state)
        {
            if (LocationUpdateMuted || _actNodePos == null) return;

            Dispatcher?.Invoke(() =>
            {
                foreach (var item in _actNodePos)
                {
                    OnNodeLocationChanged?.Invoke(item.Key, item.Value);
                }
            });
        }

        public static bool LocationUpdateMuted { private get; set; }

        public static Dispatcher Dispatcher { get; set; }

        public static void AddNode(NodeViewModel node, Point location)
        {
            OnAddNode?.Invoke(node, location);
        }

        public static void RemoveNode(NodeViewModel node)
        {
            OnRemoveNode?.Invoke(node);

            Dispatcher?.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (_actNodePos.ContainsKey(node))
                    _actNodePos.Remove(node);
            }));
        }

        public static void NodeLocationChanged(NodeViewModel node, Point location)
        {
            Dispatcher?.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!_actNodePos.ContainsKey(node))
                    _actNodePos.Add(node, location);

                _actNodePos[node] = location;
            }));
        }

        public static void AddConnection(ConnectionViewModel connection)
        {
            OnAddConnection?.Invoke(connection);
        }

        public static void RemoveConnection(ConnectionViewModel connection)
        {
            OnRemoveConnection?.Invoke(connection);
        }

        public static void NotifyConnectRequested(bool isConnecting, NodeViewModel sourceNode, IConnectorData connData)
        {
            OnConnectRequested?.Invoke(isConnecting, sourceNode, connData);
        }

        public static void ShiftZOrder(NodeViewModel node, bool up)
        {
            OnSetZIndex?.Invoke(node, up);
        }

        public static void CreateConnection(NodeViewModel targetNode, int connIdx)
        {
            OnCreateConnection?.Invoke(targetNode, connIdx);
        }

        public static void Dispose()
        {
            _actNodePos.Clear();

            _updateTimer.Dispose();
            Thread.Sleep(100);

            OnAddNode = null;
            OnRemoveNode = null;
            OnNodeLocationChanged = null;
            OnAddConnection = null;
            OnRemoveConnection = null;
            OnConnectRequested = null;
            OnSetZIndex = null;
            OnCreateConnection = null;

            _actNodePos = null;
            Dispatcher = null;
        }

        public static event Action<NodeViewModel, Point> OnAddNode;
        public static event Action<NodeViewModel> OnRemoveNode;
        public static event Action<NodeViewModel, Point> OnNodeLocationChanged;
        public static event Action<ConnectionViewModel> OnAddConnection;
        public static event Action<ConnectionViewModel> OnRemoveConnection;
        public static event Action<bool, NodeViewModel, IConnectorData> OnConnectRequested;
        public static event Action<NodeViewModel, bool> OnSetZIndex;

        public static Func<NodeViewModel, int, ConnectionViewModel> OnCreateConnection;
    }
}
