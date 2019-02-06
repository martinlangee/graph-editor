using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    // todo: ConnectorLines einführen

    public class EditorAreaViewModel: BaseNotification
    {
        private Point _currentMouse;

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
            var newNodeVm = new GraphNodeViewModel(this, _currentMouse);
            initNode?.Invoke(newNodeVm);
            GraphNodes.Add(newNodeVm);

            return newNodeVm;
        }

        public void RemoveNode(GraphNodeViewModel graphNodeVm)
        {
            GraphNodes.Remove(graphNodeVm);
        }

        public void SetCurrentMouse(Point point)
        {
            _currentMouse = point;
        }
    }
}
