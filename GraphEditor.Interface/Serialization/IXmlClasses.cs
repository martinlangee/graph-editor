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
