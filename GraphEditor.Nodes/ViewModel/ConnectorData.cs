using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;

namespace GraphEditor.Nodes.ViewModel
{
    public class ConnectorData : BaseNotification, IConnectorData
    {
        private bool _isActive;
        private Func<IConnectorData, bool> _checkIsConnected;

        public ConnectorData(string name, int index, bool isOutBound, bool isActive, Func<IConnectorData, bool> checkIsConnected, object type = null)
        {
            _checkIsConnected = checkIsConnected;
            Name = name;
            Index = index;
            IsOutBound = isOutBound;
            IsActive = isActive;
            Type = type;
        }

        public string Name { get; }

        public int Index { get; }

        public bool IsOutBound { get; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (!_checkIsConnected(this))
                    SetProperty<ConnectorData, bool>(ref _isActive, value, nameof(IsActive));
            }
        }

        public object Type { get; }  // TDOD: später semantischen Typ einführen für Prüfung: wer kann mit wem verbunden werden?
    }

}
