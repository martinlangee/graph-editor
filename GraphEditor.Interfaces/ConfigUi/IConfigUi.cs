using System;
using System.Windows.Controls;

namespace GraphEditor.Interfaces.ConfigUi
{
    public interface IConfigUi
    {
        event Action<UserControl> OnClose;
    }
}
