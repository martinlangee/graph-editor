using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Interfaces.Nodes
{
    public interface INodeTypeRepository
    {
        IList<INodeTypeData> NodeTypes { get; }

        INodeTypeData Find(string type);
    }
}
