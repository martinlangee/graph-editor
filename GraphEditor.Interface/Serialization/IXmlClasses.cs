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
