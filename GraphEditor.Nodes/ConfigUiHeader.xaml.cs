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
        public ConfigUiHeader()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }

        public event Action OnClose;
    }
}
