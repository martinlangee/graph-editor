using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    public class GraphNodeViewModel: BaseNotification
    {
        private string _selectedOutConnCount = "1";
        private string _selectedInConnCount = "1";
        private bool _isSelected = false;

        public GraphNodeViewModel(EditorAreaViewModel area, Point pos)
        {
            InConnectorCount = new ObservableCollection<string>();
            InConnectors = new ObservableCollection<int>();

            OutConnectorCount = new ObservableCollection<string>();
            OutConnectors = new ObservableCollection<int>();

            Area = area;

            var graphNode = new GraphNode { DataContext = this };
            Area.Canvas.Children.Add(graphNode);
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

        public EditorAreaViewModel Area { get; }

        public RelayCommand RemoveCommand { get; }

        public string Type { get; set; } = "Filter";
        public string Name { get; set; } = "Neu";

        public ObservableCollection<string> InConnectorCount { get; }

        public string SelectedInConnCount
        {
            get { return _selectedInConnCount; }
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
            get { return _selectedOutConnCount; }
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

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value, nameof(IsSelected)); }
        }

        public void Remove()
        {
            var toDelete = Area.Canvas.Children.OfType<GraphNode>().FirstOrDefault(gn => gn.DataContext.Equals(this));
            Area.Canvas.Children.Remove(toDelete);

            Area.RemoveNode(this);
        }
    }
}
