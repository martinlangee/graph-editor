﻿#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
// License for the specific language governing rights and limitations under the License.
#endregion

using GraphEditor.Interface.Container;
using GraphEditor.Interface.Nodes;
using GraphEditor.Ui.ViewModel;
using GraphEditor.MyNodes.Types;
using GraphEditor.Interface.Serialization;
using GraphEditor.Interface.Ui;

namespace GraphEditor.Bl
{
    public class BootStrapper
    {
        public static void InitServices()
        {
            using (ServiceContainer.RegisterTransaction())
            {
                ServiceContainer.Register<BaseXmlClasses, IXmlClasses>();
                ServiceContainer.Register<NodeTypeRepository, INodeTypeRepository>();
                ServiceContainer.Register<AreaViewModel, IAreaViewModel>();
            }
        }

        public static void FinalizeServices()
        {
            ServiceContainer.FinalizeServices();
        }
    }
}
