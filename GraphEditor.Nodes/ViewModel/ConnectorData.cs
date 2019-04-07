using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;

namespace GraphEditor.Nodes.ViewModel
{
    public class ConnectorData : BaseNotification, IConnectorData
    {
        private bool _isActive;
        private readonly Action<IConnectorData, bool> _onIsActiveChanged;
        private readonly Func<IConnectorData, bool> _canBeDeactivated;

        public ConnectorData(string name, int index, bool isOutBound, bool isActive, Action<IConnectorData, bool> onIsActiveChanged, Func<IConnectorData, bool> canBeDeactivated, object type = null)
        {
            _canBeDeactivated = canBeDeactivated;

            Name = name;
            Index = index;
            IsOutBound = isOutBound;
            IsActive = isActive;
            Type = type;

            _onIsActiveChanged = onIsActiveChanged;
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
                        (connData, isActive) =>
                            {
                                _onIsActiveChanged?.Invoke(connData, isActive);
                            });
                }
            }
        }

        public object Type { get; }  // TDOD: später semantischen Typ einführen für Prüfung: wer kann mit wem verbunden werden?
    }

}
