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
        private string _selectedOutConnCount = "1";
        private string _selectedInConnCount = "1";

        public GraphNodeViewModel(EditorAreaViewModel area, Point pos)
        {
            InConnectorCount = new ObservableCollection<string>();
            InConnectors = new ObservableCollection<int>();

            OutConnectorCount = new ObservableCollection<string>();
            OutConnectors = new ObservableCollection<int>();

            _area = area;

            var graphNode = new GraphNode { DataContext = this };
            _area.Canvas.Children.Add(graphNode);
            Canvas.SetLeft(graphNode, pos.X);
            Canvas.SetTop(graphNode, pos.Y);

            RemoveCommand = new RelayCommand(o => Remove());

            for (var c = 1; c <= 9; c++)
            {
                InConnectorCount.Add(c.ToString());
                OutConnectorCount.Add(c.ToString());
            }

            InConnectors.Add(1);
            OutConnectors.Add(1);
        }

        public RelayCommand RemoveCommand { get; }

        public string Type { get; set; } = "Filter";
        public string Name { get; set; } = "Neu";

        public ObservableCollection<string> InConnectorCount { get; }

        public string SelectedInConnCount
        {
            get => _selectedInConnCount;
            set
            {
                if (_selectedInConnCount == value) return;

                _selectedInConnCount = value;
                InConnectors.Clear();
                for (var c = 1; c <= int.Parse(value); c++)
                    InConnectors.Add(c);
            }
        }

        public ObservableCollection<int> InConnectors { get; }

        public ObservableCollection<string> OutConnectorCount { get; }

        public string SelectedOutConnCount
        {
            get => _selectedOutConnCount;
            set
            {
                if (_selectedOutConnCount == value) return;

                _selectedOutConnCount = value;
                OutConnectors.Clear();
                for (var c = 1; c <= int.Parse(value); c++)
                    OutConnectors.Add(c);
            }
        }

        public ObservableCollection<int> OutConnectors { get; }

        public void Remove()
        {
            var toDelete = _area.Canvas.Children.OfType<GraphNode>().FirstOrDefault(gn => gn.DataContext.Equals(this));
            _area.Canvas.Children.Remove(toDelete);

            _area.RemoveNode(this);
        }
    }
}
