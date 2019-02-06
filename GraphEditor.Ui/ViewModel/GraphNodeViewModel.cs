using System.Collections.ObjectModel;
using System.Linq;

namespace GraphEditor.ViewModel
{
    // todo: remove GraphNode

    public class GraphNodeViewModel: BaseNotification
    {
        private readonly EditorAreaViewModel _area;

        public GraphNodeViewModel(EditorAreaViewModel area)
        {
            _area = area;

            _area.Canvas.Children.Add(new GraphNode { DataContext = this });

            RemoveCommand = new RelayCommand(o => Remove());
        }

        public RelayCommand RemoveCommand { get; }

        public string Type { get; set; }
        public string Name { get; set; }

        public int Height { get; set; } = 100;
        public int Width { get; set; } = 50;

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
