using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace GraphEditor.ViewModel
{
    /// Todo: ConnectionLines einführen

    public class AreaViewModel: BaseNotification
    {
        private readonly Action<NodeViewModel> _onAddNode;
        private readonly Action<NodeViewModel> _onRemoveNode;
        private readonly Action<NodeViewModel, Point> _onNodeLocationChanged;
        private readonly Action<ConnectionViewModel> _onAddConnection;
        private readonly Action<NodeViewModel> _onUpdateConnections;
        private readonly Action<ConnectionViewModel> _onRemoveConnection;

        public AreaViewModel(Action<NodeViewModel> onAddNode, Action<NodeViewModel> onRemoveNode,
            Action<NodeViewModel, Point> onNodeLocationChanged,
            Action<ConnectionViewModel> onAddConnection, 
            Action<NodeViewModel> onUpdateConnections,
            Action<ConnectionViewModel> onRemoveConnection)
        {
            _onAddNode = onAddNode;
            _onRemoveNode = onRemoveNode;
            _onNodeLocationChanged = onNodeLocationChanged;
            _onAddConnection = onAddConnection;
            _onUpdateConnections = onUpdateConnections;
            _onRemoveConnection = onRemoveConnection;

            NodeVMs = new ObservableCollection<NodeViewModel>();
            AddNodeCommand = new RelayCommand(o => AddNodeExec());
        }

        public ObservableCollection<NodeViewModel> NodeVMs { get; set; }

        public RelayCommand AddNodeCommand { get; }

        public NodeViewModel AddNodeExec(Action<NodeViewModel> initNode = null)
        {
            var newNodeVm = new NodeViewModel(this, _onNodeLocationChanged, _onAddConnection, _onUpdateConnections, _onRemoveConnection);
            initNode?.Invoke(newNodeVm);
            NodeVMs.Add(newNodeVm);

            _onAddNode(newNodeVm);

            return newNodeVm;
        }

        public void RemoveNode(NodeViewModel nodeVm)
        {
            NodeVMs.Remove(nodeVm);
            _onRemoveNode(nodeVm);
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

        internal bool AnyFreeInputsFor(NodeViewModel nodeVm)
        {
            //NodeVMs.Any(gn => gn.InConnectors.Any(conn => conn.)
            return true;
        }
    }
}
