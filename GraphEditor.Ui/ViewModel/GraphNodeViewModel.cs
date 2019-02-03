using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.ViewModel
{
    // todo: remove GraphNode

    public class GraphNodeViewModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }


        public ObservableCollection<int> InConnectors { get; }
        public ObservableCollection<int> OutConnectors { get; }

    }
}
