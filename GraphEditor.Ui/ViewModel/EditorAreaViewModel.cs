using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            GraphNodeVMs = new ObservableCollection<GraphNodeViewModel>();
            AddNodeCommand = new RelayCommand(o => Add());
        }

        public ObservableCollection<GraphNodeViewModel> GraphNodeVMs { get; set; }

        public RelayCommand AddNodeCommand { get; }

        internal Canvas Canvas { get; }

        public GraphNodeViewModel Add(Action<GraphNodeViewModel> initNode = null)
        {
            var newNodeVm = new GraphNodeViewModel(this, _currentMouse);
            initNode?.Invoke(newNodeVm);
            GraphNodeVMs.Add(newNodeVm);

            return newNodeVm;
        }

        public void RemoveNode(GraphNodeViewModel graphNodeVm)
        {
            GraphNodeVMs.Remove(graphNodeVm);
        }

        public void SetCurrentMouse(Point point)
        {
            _currentMouse = point;
        }

        public List<GraphNodeViewModel> Selected
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
