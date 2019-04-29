#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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
