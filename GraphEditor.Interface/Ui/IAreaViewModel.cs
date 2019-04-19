using System.Collections.Generic;

namespace GraphEditor.Interface.Ui
{
    public interface IAreaViewModel
    {
        IToolBarViewModel ToolBar { get; }

        IList<INodeViewModel> Selected { get; }

        void RevokeConnectRequestStatus();

        void DeselectAll();
    }
}
