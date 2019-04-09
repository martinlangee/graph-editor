using GraphEditor.Interface.ConfigUi;
using System;

namespace GraphEditor.Interface.Nodes
{
    public class ConnectorData : BaseNotification, IConnectorData
    {
        private bool _isActive;
        private readonly Action<IConnectorData> _onIsActiveChanged;
        private readonly Func<IConnectorData, bool> _canBeDeactivated;

        public ConnectorData(string name, int index, bool isOutBound, bool isActive, Action<IConnectorData> onIsActiveChanged, Func<IConnectorData, bool> canBeDeactivated, byte[] icon = null, object type = null)
        {
            _canBeDeactivated = canBeDeactivated;

            Name = name;
            Index = index;
            IsOutBound = isOutBound;
            IsActive = isActive;
            Type = type;
            Icon = icon;

            _onIsActiveChanged = onIsActiveChanged;  // ! must be assigned only after IsActive is set
        }

        public string Name { get; }

        public int Index { get; }

        public bool IsOutBound { get; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_canBeDeactivated(this) || value)
                {
                    SetProperty<ConnectorData, bool>(ref _isActive, value, nameof(IsActive),
                        (connData, isActive) => _onIsActiveChanged?.Invoke(connData));
                }
            }
        }

        public object Type { get; }  // TDOD: später semantischen Typ einführen für Prüfung: wer kann mit wem verbunden werden?

        public byte[] Icon { get; }
    }

}
