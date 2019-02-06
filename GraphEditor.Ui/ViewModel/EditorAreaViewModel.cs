using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    // todo: Add GraphNode
    // todo: Shift GraphNode
    // todo: ConnectorLines einführen

    public class EditorAreaViewModel: BaseNotification
    {
        public EditorAreaViewModel(Canvas canvas)
        {
            Canvas = canvas;

            GraphNodes = new ObservableCollection<GraphNodeViewModel>();
            AddNodeCommand = new RelayCommand(o => Add());
        }

        public ObservableCollection<GraphNodeViewModel> GraphNodes { get; set; }

        public RelayCommand AddNodeCommand { get; }

        internal Canvas Canvas { get; }

        public GraphNodeViewModel Add(Action<GraphNodeViewModel> initNode = null)
        {
            var newNodeVm = new GraphNodeViewModel(this);
            initNode?.Invoke(newNodeVm);
            GraphNodes.Add(newNodeVm);

            return newNodeVm;
        }

        public void RemoveNode(GraphNodeViewModel graphNodeVm)
        {
            GraphNodes.Remove(graphNodeVm);
        }
    }
}
