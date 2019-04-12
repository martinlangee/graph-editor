using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Interface.Serialization
{
    public class BaseXmlClasses : IXmlClasses
    {
        public virtual string Root => nameof(Root);

        public virtual string Nodes => nameof(Nodes);

        public virtual string Node => nameof(Node);

        public virtual string Location => nameof(Location);

        public virtual string Id => nameof(Id);

        public virtual string Name => nameof(Name);

        public virtual string Type => nameof(Type);

        public virtual string Inputs => nameof(Inputs);

        public virtual string Outputs => nameof(Outputs);

        public virtual string Slot => nameof(Slot);

        public virtual string Index => nameof(Index);

        public virtual string Active => nameof(Active);

        public virtual string Specific => nameof(Specific);

        public virtual string Connections => nameof(Connections);

        public virtual string Connection => nameof(Connection);

        public virtual string Source => nameof(Source);

        public virtual string SourceConn => nameof(SourceConn);

        public virtual string Target => nameof(Target);

        public virtual string TargetConn => nameof(TargetConn);

        public virtual string Points => nameof(Points);
    }
}
