using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GraphEditor.ViewModel
{
    // todo: remove GraphNode

    public class GraphNodeViewModel
    {
        private EditorAreaViewModel _area;
        private int _x;

        public GraphNodeViewModel(EditorAreaViewModel area)
        {
            _area = area;
        }

        public string Type { get; set; }
        public string Name { get; set; }

        public int X
        {
            get { return _x; }
            set
            {
                _x = value;
            }
        }

        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public ObservableCollection<int> InConnectors { get; }
        public ObservableCollection<int> OutConnectors { get; }

        public void Remove()
        {
            _area.GraphNodes.Remove(this);
        }


    }
}
