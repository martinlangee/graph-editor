using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    // todo: Add GraphNode
    // todo: Shift GraphNode
    // todo: ConnectorLines einführen

    public class EditorAreaViewModel
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
            var newNode = new GraphNodeViewModel(this);
            initNode?.Invoke(newNode);
            GraphNodes.Add(newNode);

            Canvas.Children.Add(new GraphNode());

            return newNode;
        }
    }
}
