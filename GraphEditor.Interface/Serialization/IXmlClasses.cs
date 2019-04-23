#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using System.Collections.Generic;
using System.Xml.Linq;

namespace GraphEditor.Interface.Serialization
{
    public interface IXmlClasses
    {
        string Root { get; }
        string Nodes { get; }
        string Node { get; }
        string Location { get; }
        string Id { get; }
        string Name { get; }
        string Type { get; }
        string Inputs { get; }
        string Outputs { get; }
        string Slot { get; }
        string Index { get; }
        string Active { get; }
        string Specific { get; }
        string Connections { get; }
        string Connection { get; }
        string Source { get; }
        string SourceConn { get; }
        string Target { get; }
        string TargetConn { get; }
        string Points { get; }
        string Param { get; }
        string Value { get; }

        IDictionary<string, string> GetParamValues(XElement paramParentXml, params string[] paramNames);
    }
}
