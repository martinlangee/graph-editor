using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.ViewModel
{
    // todo: Add GraphNode
    // todo: Shift GraphNode
    // todo: ConnectorLines einführen

    public class EditorAreaViewModel
    {
        public EditorAreaViewModel()
        {
            GraphNodes = new ObservableCollection<GraphNodeViewModel>();
        }

        public ObservableCollection<GraphNodeViewModel> GraphNodes { get; }
    }
}
