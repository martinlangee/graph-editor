using GraphEditor.Tools;
using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class NodeViewModel : BaseNotification
    {
        private bool _isSelected = false;
        private Point _location;

        public NodeViewModel(AreaViewModel area)
        {
            Area = area;

            InConnectors = new ObservableCollection<ConnectorStateViewModel>();
            OutConnectors = new ObservableCollection<ConnectorStateViewModel>();

            OutConnections = new ObservableCollection<ConnectionViewModel>();

            ConnectToCommand = new RelayCommand(o => ConnectToExec(), CanExecuteConnectTo);
            RemoveNodeCommand = new RelayCommand(o => RemoveExec());

            // todo: provisorisch
            for (int i = 0; i < 5; i++)
                InConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: false));

            for (int i = 0; i < 5; i++)
                OutConnectors.Add(new ConnectorStateViewModel(this, i, isOutBound: true));
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

        public ObservableCollection<ConnectorStateViewModel> InConnectors { get; }

        public ObservableCollection<ConnectionViewModel> OutConnections { get; }

        public void AddOutConnection(int sourceConn, NodeViewModel targetConnVm, int targetConn)
        {
            var connVm = new ConnectionViewModel(this, targetConnVm, sourceConn, targetConn);
            OutConnections.Add(connVm);
            MessageHub.Inst.AddConnection(connVm);
        }

        public ObservableCollection<ConnectorStateViewModel> OutConnectors { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty<NodeViewModel, bool>(ref _isSelected, value, nameof(IsSelected)); }
        }

        public Point Location
        {
            get => _location;
            set
            {
                SetProperty<NodeViewModel, Point>(ref _location, value, nameof(Location),
                    (vm, pt) => MessageHub.Inst.NodeLocationChanged(vm, pt));
            }
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
