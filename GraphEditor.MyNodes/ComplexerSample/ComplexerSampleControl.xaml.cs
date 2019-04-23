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
using GraphEditor.Interface.Ui;
using System.Windows.Controls;

namespace GraphEditor.MyNodes.ComplexerSample
{
    /// <summary>
    /// Interaktionslogik für StartUpWarningControl.xaml
    /// </summary>
    public partial class ComplexerSampleControl : UserControl, INodeConfigUi
    {
        public ComplexerSampleControl(INodeData nodeData)
        {
            InitializeComponent();

            DataContext = nodeData;
        }

        public string Title => (DataContext as INodeData).TypeData.Name;
    }
}