using System;
using System.Windows.Controls;

namespace GraphEditor.Interfaces.ConfigUi
{
    public interface INodeConfigUi
    {
        event Action<UserControl> OnClose;
    }
}
