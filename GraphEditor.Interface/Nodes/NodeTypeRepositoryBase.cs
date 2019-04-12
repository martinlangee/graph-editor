using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor.Interface.Nodes
{
    public abstract class NodeTypeRepositoryBase : INodeTypeRepository
    {
        protected abstract void CreateNodeTypes();

        public IList<IBaseNodeTypeData> NodeTypes { get; } = new List<IBaseNodeTypeData>();

        public IBaseNodeTypeData Find(string type)
        {
            return NodeTypes.FirstOrDefault(nt => nt.Type.Equals(type));
        }

        // called by IoC container
        public virtual void OnBuiltUp()
        {
            CreateNodeTypes();
        }

        // called by IoC container
        public virtual void TearDown()
        {
        }
    }
}
