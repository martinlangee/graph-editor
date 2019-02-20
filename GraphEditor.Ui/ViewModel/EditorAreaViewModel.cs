using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    /// Todo: ConnectionLines einführen

    public class EditorAreaViewModel: BaseNotification
    {
        private readonly Action<NodeViewModel> _onAddGraphNode;
        private readonly Action<NodeViewModel> _onRemoveGraphNode;

        private readonly Action<ConnectionViewModel> _onAddConnection;
        private readonly Action<ConnectionViewModel> _onRemoveConnection;

        public EditorAreaViewModel(Action<NodeViewModel> onAddGraphNode, Action<NodeViewModel> onRemoveGraphNode,
            Action<ConnectionViewModel> onAddConnection, Action<ConnectionViewModel> onRemoveConnection)
        {
            _onAddGraphNode = onAddGraphNode;
            _onRemoveGraphNode = onRemoveGraphNode;
            _onAddConnection = onAddConnection;
            _onRemoveConnection = onRemoveConnection;

            GraphNodeVMs = new ObservableCollection<NodeViewModel>();
            AddNodeCommand = new RelayCommand(o => AddNodeExec());
        }

        public ObservableCollection<NodeViewModel> GraphNodeVMs { get; set; }

        public RelayCommand AddNodeCommand { get; }

        public NodeViewModel AddNodeExec(Action<NodeViewModel> initNode = null)
        {
            var newNodeVm = new NodeViewModel(this, _onAddConnection, _onRemoveConnection);
            initNode?.Invoke(newNodeVm);
            GraphNodeVMs.Add(newNodeVm);

            _onAddGraphNode(newNodeVm);

            return newNodeVm;
        }

        public void RemoveNode(NodeViewModel graphNodeVm)
        {
            GraphNodeVMs.Remove(graphNodeVm);
            _onRemoveGraphNode(graphNodeVm);
        }

        public List<NodeViewModel> Selected
        {
            get { return GraphNodeVMs.Where(graphNodeViewModel => graphNodeViewModel.IsSelected).ToList(); }
        }

        public int SelectedCount
        {
            get { return GraphNodeVMs.Sum(gn => gn.IsSelected ? 1 : 0); }
        }

        public void DeselectAll()
        {
            foreach (var graphNodeViewModel in GraphNodeVMs)
            {
                graphNodeViewModel.IsSelected = false;
            }
        }
    }
}
