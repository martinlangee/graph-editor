#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
// License for the specific language governing rights and limitations under the License.
#endregion

using GraphEditor.Interface.Nodes;
using GraphEditor.MyNodes.LogicalAND;
using GraphEditor.MyNodes.LogicalXOR;
using GraphEditor.MyNodes.LogicalOR;
using GraphEditor.MyNodes.ComplexerSample;

namespace GraphEditor.MyNodes.Types
{
    public class NodeTypeRepository : NodeTypeRepositoryBase
    {
        protected override void CreateNodeTypes()
        {
            // adding some sample NodeTypes to the repository
            NodeTypes.Add(new LogicalANDType());
            NodeTypes.Add(new LogicalORType());
            NodeTypes.Add(new LogicalXORType());
            NodeTypes.Add(new ComplexerSampleType());
        }
    }
}
