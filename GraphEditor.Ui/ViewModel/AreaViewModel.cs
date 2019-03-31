using GraphEditor.Ui.Commands;
using GraphEditor.Ui.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphEditor.Ui.ViewModel
{
    public class AreaViewModel : BaseNotification
    {
        private struct ConnectingNodeData
        {
            internal NodeViewModel SourceNode;
            internal int ConnIdx;
            internal bool IsOutBound;
        }

        ConnectingNodeData _connNode;

        public AreaViewModel()
        {
            ToolBar = new ToolBarViewModel();
            NodeVMs = new ObservableCollection<NodeViewModel>();

            AddNodeCommand = new RelayCommand(o => AddNodeExec());

            UiMessageHub.OnRemoveNode += OnRemoveNode;
            UiMessageHub.OnConnectRequested += OnConnectRequested;
            UiMessageHub.OnCreateConnection += OnCreateConnection;
        }

        // called by IoC container
        public void ShutDown()
        {
            UiMessageHub.Dispose();
        }

        public ObservableCollection<NodeViewModel> NodeVMs { get; set; }

        public RelayCommand AddNodeCommand { get; }

        public NodeViewModel AddNodeExec(Action<NodeViewModel> initNode = null)
        {
            var newNodeVm = new NodeViewModel(() => NodeVMs.ToList());
            initNode?.Invoke(newNodeVm);
            NodeVMs.Add(newNodeVm);

            UiMessageHub.AddNode(newNodeVm);

            return newNodeVm;
        }

        public void OnRemoveNode(NodeViewModel nodeVm)
        {
            NodeVMs.Remove(nodeVm);
        }

        public void RevokeConnectRequestStatus()
        {
            if (_connNode.SourceNode != null)
            {
                if (_connNode.IsOutBound)
                    _connNode.SourceNode.OutConnectors[_connNode.ConnIdx].IsConnecting = false;
                else
                    _connNode.SourceNode.InConnectors[_connNode.ConnIdx].IsConnecting = false;
            }
        }

        private void UpdateConnectingNodeData(bool isConnecting, NodeViewModel sourceNode, int connIdx, bool isOutBound)
        {
            if (isConnecting)
            {
                _connNode.SourceNode = sourceNode;
                _connNode.ConnIdx = connIdx;
                _connNode.IsOutBound = isOutBound;
            }
            else
                _connNode.SourceNode = null;
        }

        private void OnConnectRequested(bool isConnecting, NodeViewModel sourceNode, int connIdx, bool isOutBound)
        {
            // first deactivate the connecting status of a former connecting node
            RevokeConnectRequestStatus();

            NodeVMs.Where(node => !node.Equals(sourceNode)).ToList().ForEach(node => node.ConnectRequested(isConnecting, sourceNode, connIdx, isOutBound));

            UpdateConnectingNodeData(isConnecting, sourceNode, connIdx, isOutBound);
        }

        private void OnCreateConnection(NodeViewModel targetNode, int connIdx)
        {
            if (_connNode.SourceNode != null)
            {
                if (_connNode.IsOutBound)
                {
                    _connNode.SourceNode.AddOutConnection(_connNode.ConnIdx, targetNode, connIdx);
                }
                else
                {
                    targetNode.AddOutConnection(connIdx, _connNode.SourceNode, _connNode.ConnIdx);
                }
                RevokeConnectRequestStatus();
            }
        }

        public List<NodeViewModel> Selected
        {
            get { return NodeVMs.Where(nodeVm => nodeVm.IsSelected).ToList(); }
        }

        public int SelectedCount
        {
            get { return NodeVMs.Sum(gn => gn.IsSelected ? 1 : 0); }
        }

        public void DeselectAll()
        {
            foreach (var nodeVm in NodeVMs)
            {
                nodeVm.IsSelected = false;
            }
        }

        public ToolBarViewModel ToolBar { get; }
    }
}
