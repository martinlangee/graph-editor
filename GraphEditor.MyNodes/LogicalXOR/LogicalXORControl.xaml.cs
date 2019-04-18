﻿using GraphEditor.Interface.ConfigUi;
using GraphEditor.Interface.Nodes;
using System.Windows.Controls;

namespace GraphEditor.MyNodes.LogicalXOR
{
    /// <summary>
    /// Interaktionslogik für LogicalXORControl.xaml
    /// </summary>
    public partial class LogicalXORControl : UserControl, INodeConfigUi
    {
        public LogicalXORControl(INodeData nodeData)
        {
            InitializeComponent();

            DataContext = nodeData;
        }

        public string Title => (DataContext as INodeData).TypeData.Name;
    }
}
