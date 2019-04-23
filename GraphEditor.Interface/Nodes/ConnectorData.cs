#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
// License for the specific language governing rights and limitations under the License.
#endregion

using GraphEditor.Interface.Ui;
using System;

namespace GraphEditor.Interface.Nodes
{
    public class ConnectorData<T> : BaseNotification, IConnectorData
    {
        private bool _isActive = true;
        private readonly Action<IConnectorData> _onIsActiveChanged;
        private readonly Func<IConnectorData, bool> _canBeDeactivated;
        private byte[] _icon;

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

        public event Action IconChanged;

        public byte[] Icon { get => _icon; set => SetProperty<ConnectorData<T>, byte[]>(ref _icon, value, nameof(Icon), (c, v) => IconChanged?.Invoke()); }

        public uint Color { get; }
    }
}
