﻿using GraphEditor.Interface.Nodes;
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