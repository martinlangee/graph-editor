using System;
using System.Windows.Controls;

namespace GraphEditor.Interface.ConfigUi
{
    public interface INodeConfigUi
    {
        event Action<INodeConfigUi> OnClose;
    }
}
