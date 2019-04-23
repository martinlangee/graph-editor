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
using GraphEditor.Interface.Utils;
using GraphEditor.Ui.Tools;
using System;
using System.Windows.Media;

namespace GraphEditor.Ui.ViewModel
{
    public class OutConnectorViewModel : ConnectorViewModel
    {
        private OutConnectorViewModel(NodeViewModel nodeVm, string name, int index) : base(nodeVm, name, index)
        {
            Brush = new SolidColorBrush(_nodeVm.Data.Outs[Index].Color.ToColor());

            _nodeVm.Data.Outs[Index].IconChanged += () => FirePropertiesChanged(nameof(Icon));
        }

        public static ConnectorViewModel Create(NodeViewModel nodeVm, string name, int index)
        {
            return new OutConnectorViewModel(nodeVm, name, index);
        }

        protected override void NotifyConnectRequested(bool isConnecting, NodeViewModel nodeVm, int index)
        {
            UiMessageHub.NotifyConnectRequested(isConnecting, nodeVm, _nodeVm.Data.Outs[index]);
        }

        public override byte[] Icon => _nodeVm.Data.Outs[Index].Icon;

        public override Brush Brush { get ; }

        public override bool IsOutBound => true;
    }
}
