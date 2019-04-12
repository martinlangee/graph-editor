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
        private Func<T, Color> _typeToColor;

        public ConnectorData(string name, int index, bool isOutBound, Action<IConnectorData> onIsActiveChanged, Func<IConnectorData, bool> canBeDeactivated, T type, byte[] icon = null, Func<T, Color> typeToColor = null)
        {
            Name = name;
            Index = index;
            IsOutBound = isOutBound;
            Type = type;
            Icon = icon;

            _canBeDeactivated = canBeDeactivated;
            _onIsActiveChanged = onIsActiveChanged;
            _typeToColor = typeToColor == null ? T => Colors.CornflowerBlue : typeToColor;
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

        public Color TypeAsColor => _typeToColor((T) Type);
    }
}
