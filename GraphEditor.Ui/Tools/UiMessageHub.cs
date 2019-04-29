#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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
            Dispatcher?.Invoke(() =>
            {
                if (LocationUpdateMuted || _actNodePos == null) return;

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
            _actNodePos = null;

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
