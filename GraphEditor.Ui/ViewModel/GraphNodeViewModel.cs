using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    public class GraphNodeViewModel: BaseNotification
    {
        private string _selectedOutConnectorCount = "1";
        private string _selectedInConnectorCount = "1";
        private bool _isSelected = false;

        public GraphNodeViewModel(EditorAreaViewModel area)
        {
            InConnectorCount = new ObservableCollection<string>();
            InConnectors = new ObservableCollection<int>();

            OutConnectorCount = new ObservableCollection<string>();
            OutConnectors = new ObservableCollection<int>();

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            Area = area;

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

        public string SelectedInConnectorCount
        {
            get { return _selectedInConnectorCount; }
            set
            {
                if (_selectedInConnectorCount == value) return;

                _selectedInConnectorCount = value;
                InConnectors.Clear();
                for (var c = 1; c <= int.Parse(value); c++)
                    InConnectors.Add(c);
            }
        }

        public ObservableCollection<int> InConnectors { get; }

        public ObservableCollection<string> OutConnectorCount { get; }

        public ObservableCollection<ConnectionViewModel> OutConnections { get; }

        public void AddOutConnection(int sourceConn)
        {
            OutConnections.Add(new ConnectionViewModel(this, sourceConn));
        }

        public string SelectedOutConnectorCount
        {
            get { return _selectedOutConnectorCount; }
            set
            {
                if (_selectedOutConnectorCount == value) return;

                _selectedOutConnectorCount = value;
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
            Area.RemoveNode(this);
        }
    }
}
