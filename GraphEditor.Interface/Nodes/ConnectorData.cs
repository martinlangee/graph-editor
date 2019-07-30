#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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
                if (CanBeDeactivated || value)
                {
                    SetProperty<ConnectorData<T>, bool>(ref _isActive, value, nameof(IsActive),
                        (connData, isActive) => _onIsActiveChanged?.Invoke(connData));
                }
            }
        }

        public bool CanBeDeactivated => _canBeDeactivated(this);

        public object Type { get; }

        public event Action IconChanged;

        public byte[] Icon { get => _icon; set => SetProperty<ConnectorData<T>, byte[]>(ref _icon, value, nameof(Icon), (c, v) => IconChanged?.Invoke()); }

        public uint Color { get; }
    }
}
