using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;

namespace GraphEditor.Nodes.StartUpWarning
{
    /// <summary>
    /// Interaktionslogik für StartUpWarningControl.xaml
    /// </summary>
    public partial class StartUpWarningControl : UserControl, INodeConfigUi
    {
        public StartUpWarningControl(INodeData nodeData)
        {
            InitializeComponent();

            Header.Init(() => OnClose?.Invoke(this), nodeData);

            DataContext = nodeData;
        }

        public event Action<INodeConfigUi> OnClose;
    }
}
