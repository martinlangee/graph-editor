using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
using System;
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

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
