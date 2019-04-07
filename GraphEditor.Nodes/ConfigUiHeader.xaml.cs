using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GraphEditor.Nodes
{
    /// <summary>
    /// Interaction logic for ConfigUiHeader.xaml
    /// </summary>
    public partial class ConfigUiHeader : UserControl
    {
        Action _onClose;

        public ConfigUiHeader()
        {
            InitializeComponent();
        }

        public void Init(Action onClose, INodeData nodeData)
        {
            _onClose = onClose;
            _tbHeader.Text = $"Configuration {nodeData.TypeData.Name}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _onClose?.Invoke();
        }
    }
}
