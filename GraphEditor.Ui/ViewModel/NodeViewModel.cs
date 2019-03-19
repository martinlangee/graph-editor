using GraphEditor.Tools;
using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class NodeViewModel : BaseNotification
    {
        private string _selectedOutConnectorCount = "5";
        private string _selectedInConnectorCount = "5";
        private bool _isSelected = false;
        private Point _location;
        //private Action<NodeViewModel, Point> _onLocationChanged;
        //private readonly Action<ConnectionViewModel> _onAddConnection;
        //private readonly Action<NodeViewModel> _onUpdateConnections;
        //private readonly Action<ConnectionViewModel> _onRemoveConnection;

        public NodeViewModel(AreaViewModel area)
        {
            Area = area;

            InConnectorCount = new ObservableCollection<string>();
            InConnectors = new ObservableCollection<ConnectorViewModel>();

            OutConnectorCount = new ObservableCollection<string>();
            OutConnectors = new ObservableCollection<ConnectorViewModel>();

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            ConnectToCommand = new RelayCommand(o => ConnectToExec(), CanExecuteConnectTo);
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            for (var c = 1; c <= 9; c++)
            {
                InConnectorCount.Add(c.ToString());
                OutConnectorCount.Add(c.ToString());
            }

            for (int i = 0; i < 5; i++)
                InConnectors.Add(new ConnectorViewModel(this, i));

            for (int i = 0; i < 5; i++)
                OutConnectors.Add(new ConnectorViewModel(this, i));
        }

        internal void RemoveConnection(ConnectionViewModel connVm)
        {
            OutConnections.Remove(connVm);
            MessageHub.Inst.RemoveConnection(connVm);
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
                for (var idx = 0; idx < int.Parse(value); idx++)
                    InConnectors.Add(new ConnectorViewModel(this, idx));
            }
        }

        public ObservableCollection<ConnectorViewModel> InConnectors { get; }

        public ObservableCollection<string> OutConnectorCount { get; }

        public ObservableCollection<ConnectionViewModel> OutConnections { get; }

        public void AddOutConnection(int sourceConn, NodeViewModel targetConnVm, int targetConn)
        {  // todo  => ConnectorViewModel
            var connVm = new ConnectionViewModel(this, targetConnVm, sourceConn, targetConn);
            OutConnections.Add(connVm);
            MessageHub.Inst.AddConnection(connVm);
        }

        public string SelectedOutConnectorCount
        {
            get { return _selectedOutConnectorCount; }
            set
            {
                if (_selectedOutConnectorCount == value) return;

                _selectedOutConnectorCount = value;
                OutConnectors.Clear();
                for (var idx = 0; idx < int.Parse(value); idx++)
                    OutConnectors.Add(new ConnectorViewModel(this, idx));
            }
        }

        public ObservableCollection<ConnectorViewModel> OutConnectors { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty<NodeViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }
        }

        public Point Location
        {
            get => _location;
            set { SetProperty<NodeViewModel, Point>(ref _location, value, nameof(Location), 
                (vm, pt) => 
                {
                    MessageHub.Inst.NodeLocationChanged(vm, pt);
                    MessageHub.Inst.UpdateConnections(vm);
                }
                ); }
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
