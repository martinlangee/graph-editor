using GraphEditor.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphEditor.ViewModel
{
    public class AreaViewModel: BaseNotification
    {
        private struct ConnectingNodeData
        {
            internal NodeViewModel SourceNode;
            internal int ConnectorIdx;
            internal bool IsOutBound;
        }

        ConnectingNodeData _connNode;

        public AreaViewModel()
        {
            NodeVMs = new ObservableCollection<NodeViewModel>();
            AddNodeCommand = new RelayCommand(o => AddNodeExec());

            MessageHub.Inst.OnRemoveNode += OnRemoveNode;
            MessageHub.Inst.OnConnectRequested += OnConnectRequested;
        }

        public ObservableCollection<NodeViewModel> NodeVMs { get; set; }

        public RelayCommand AddNodeCommand { get; }

        public NodeViewModel AddNodeExec(Action<NodeViewModel> initNode = null)
        {
            var newNodeVm = new NodeViewModel(this);
            initNode?.Invoke(newNodeVm);
            NodeVMs.Add(newNodeVm);

            MessageHub.Inst.AddNode(newNodeVm);

            return newNodeVm;
        }

        public void OnRemoveNode(NodeViewModel nodeVm)
        {
            NodeVMs.Remove(nodeVm);
        }

        private void OnConnectRequested(bool value, NodeViewModel sourceNode, int connectorIdx, bool isOutBound)
        {
            // first deactivate the connecting status of a former connecting node
            if (_connNode.SourceNode != null)
            {
                if (_connNode.IsOutBound)
                    _connNode.SourceNode.OutConnectors[_connNode.ConnectorIdx].IsConnecting = false;
                else
                    _connNode.SourceNode.InConnectors[_connNode.ConnectorIdx].IsConnecting = false;
            }

            NodeVMs.Where(node => !node.Equals(sourceNode)).ToList().ForEach(node => node.ConnectRequested(value, sourceNode, connectorIdx, isOutBound));

            if (value)
            {
                _connNode.SourceNode = sourceNode;
                _connNode.ConnectorIdx = connectorIdx;
                _connNode.IsOutBound = isOutBound;
            }
            else
                _connNode.SourceNode = null;
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
    }
}
