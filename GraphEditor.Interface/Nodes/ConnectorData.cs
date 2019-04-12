using GraphEditor.Interface.ConfigUi;
using System;

namespace GraphEditor.Interface.Nodes
{
    public class ConnectorData<T> : BaseNotification, IConnectorData<T>
    {
        private bool _isActive = true;
        private readonly Action<IBaseConnectorData> _onIsActiveChanged;
        private readonly Func<IBaseConnectorData, bool> _canBeDeactivated;

        public ConnectorData(string name, int index, bool isOutBound, Action<IBaseConnectorData> onIsActiveChanged, Func<IBaseConnectorData, bool> canBeDeactivated, T type, byte[] icon = null)
        {
            Name = name;
            Index = index;
            IsOutBound = isOutBound;
            Type = type;
            Icon = icon;

            _canBeDeactivated = canBeDeactivated;
            _onIsActiveChanged = onIsActiveChanged;
        }

        public string Name { get; }

        public int Index { get; }

        public bool IsOutBound { get; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_canBeDeactivated(this) || value)
                {
                    SetProperty<ConnectorData<T>, bool>(ref _isActive, value, nameof(IsActive),
                        (connData, isActive) => _onIsActiveChanged?.Invoke(connData));
                }
            }
        }

        public T Type { get; }

        public byte[] Icon { get; }
    }
}
