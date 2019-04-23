﻿#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using GraphEditor.Interface.Nodes;
using System;
using System.Reflection;

namespace GraphEditor.MyNodes.LogicalOR
{
    public class LogicalORType : NodeTypeDataBase
    {
        public LogicalORType() : base(Assembly.GetExecutingAssembly())
        {
            Name = "Logical OR";
            Description = "Node representing a logical OR operation";
        }

        protected override Type NodeType => typeof(LogicalOR);
    }
}
