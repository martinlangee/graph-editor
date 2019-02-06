using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    // todo: paint connector pins

    public class GraphNodeViewModel: BaseNotification
    {
        private readonly EditorAreaViewModel _area;

        public GraphNodeViewModel(EditorAreaViewModel area, Point pos)
        {
            InConnectors = new ObservableCollection<int>();
            OutConnectors = new ObservableCollection<int>();

            _area = area;

            var graphNode = new GraphNode { DataContext = this };
            _area.Canvas.Children.Add(graphNode);
            Canvas.SetLeft(graphNode, pos.X);
            Canvas.SetTop(graphNode, pos.Y);

            RemoveCommand = new RelayCommand(o => Remove());

            InConnectors.Add(1);
            InConnectors.Add(2);
            InConnectors.Add(3);
            InConnectors.Add(4);

            OutConnectors.Add(1);
            OutConnectors.Add(2);
            OutConnectors.Add(3);
        }

        public RelayCommand RemoveCommand { get; }

        public string Type { get; set; }
        public string Name { get; set; }

        public int Height { get; set; } = 100;
        public int Width { get; set; } = 70;

        public ObservableCollection<int> InConnectors { get; }
        public ObservableCollection<int> OutConnectors { get; }

        public void Remove()
        {
            var toDelete = _area.Canvas.Children.OfType<GraphNode>().FirstOrDefault(gn => gn.DataContext.Equals(this));
            _area.Canvas.Children.Remove(toDelete);

            _area.RemoveNode(this);
        }
    }
}
