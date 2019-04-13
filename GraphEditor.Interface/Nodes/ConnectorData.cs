using GraphEditor.Interface.ConfigUi;
using System;
using System.Windows.Media;

namespace GraphEditor.Interface.Nodes
{
    public class ConnectorData<T> : BaseNotification, IConnectorData
    {
        private bool _isActive = true;
        private readonly Action<IConnectorData> _onIsActiveChanged;
        private readonly Func<IConnectorData, bool> _canBeDeactivated;

        public ConnectorData(string name, int index, bool isOutBound, Action<IConnectorData> onIsActiveChanged, Func<IConnectorData, bool> canBeDeactivated, T type, uint color, byte[] icon = null)
        {
            Name = name;
            Index = index;
            IsOutBound = isOutBound;
            Type = type;
            Icon = icon;
            Color = color;

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

        public object Type { get; }

        public byte[] Icon { get; }

        public uint Color { get; }
    }
}
