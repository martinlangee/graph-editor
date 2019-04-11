using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Interface.Nodes
{
    public interface INodeTypeRepository
    {
        IList<IBaseNodeTypeData> NodeTypes { get; }

        IBaseNodeTypeData Find(string type);
    }
}
