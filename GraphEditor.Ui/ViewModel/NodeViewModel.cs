using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class NodeViewModel : BaseNotification
    {
        private string _selectedOutConnectorCount = "1";
        private string _selectedInConnectorCount = "1";
        private bool _isSelected = false;
        private Point _location;
        private Action<NodeViewModel, Point> _onLocationChanged;
        private readonly Action<ConnectionViewModel> _onAddConnection;
        private readonly Action<ConnectionViewModel> _onRemoveConnection;

        public NodeViewModel(AreaViewModel area, Action<NodeViewModel, Point> onLocationChanged, Action<ConnectionViewModel> onAddConnection, Action<ConnectionViewModel> onRemoveConnection)
        {
            Area = area;
            _onLocationChanged = onLocationChanged;
            _onAddConnection = onAddConnection;
            _onRemoveConnection = onRemoveConnection;

            InConnectorCount = new ObservableCollection<string>();
            InConnectors = new ObservableCollection<int>();

            OutConnectorCount = new ObservableCollection<string>();
            OutConnectors = new ObservableCollection<int>();

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            ConnectToCommand = new RelayCommand(o => ConnectToExec(), CanExecuteConnectTo);
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            for (var c = 1; c <= 9; c++)
            {
                InConnectorCount.Add(c.ToString());
                OutConnectorCount.Add(c.ToString());
            }

            InConnectors.Add(1);
            OutConnectors.Add(1);
        }

        internal void RemoveConnection(ConnectionViewModel connVm)
        {
            OutConnections.Remove(connVm);
            _onRemoveConnection(connVm);
        }

        public AreaViewModel Area { get; }

        public RelayCommand ConnectToCommand { get; }
        public RelayCommand RemoveNodeCommand { get; }

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

        public void AddOutConnection(NodeViewModel targetConn, int sourceConn)
        {
            var connVm = new ConnectionViewModel(this, targetConn, sourceConn, -1);
            OutConnections.Add(connVm);
            _onAddConnection(connVm);
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
            set { SetProperty<NodeViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }
        }

        public Point Location
        {
            get => _location;
            set { SetProperty(ref _location, value, nameof(Location), _onLocationChanged); }
        }

        public void ConnectToExec()
        {
        }

        public void RemoveExec()
        {
            Area.RemoveNode(this);
        }

        private bool CanExecuteConnectTo(object obj)
        {
            return Area.AnyFreeInputsFor(this);
        }
    }
}
