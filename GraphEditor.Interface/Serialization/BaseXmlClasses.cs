#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public virtual string Param => nameof(Param);

        public virtual string Value => nameof(Value);

        public IDictionary<string,string> GetParamValues(XElement paramParentXml, params string[] paramNames)
        {
            var paramXmls = paramParentXml.Elements().Where(el => el.Name.LocalName == Param);

            var result = new Dictionary<string, string>();

            foreach (var paramName in paramNames)
            {
                result.Add(paramName, paramXmls.FirstOrDefault(el => el.Attribute(Id).Value == paramName).Attribute(Value).Value);
            }

            return result;
        }
    }
}
