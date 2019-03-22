﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphEditor.Tools;
using GraphEditor.Ui;
using GraphEditor.ViewModel;

namespace GraphEditor
{
    /// <summary>
    /// Interaktionslogik für GraphNode.xaml
    /// </summary>
    public partial class GraphNode : UserControl
    {
        public GraphNode()
        {
            InitializeComponent();
        }

        internal NodeViewModel ViewModel => (NodeViewModel)DataContext;

        internal AreaViewModel AreaVm => ViewModel.Area;

        internal EditorArea Area => (EditorArea) ((FrameworkElement) Parent).Parent;

        private Point GetConnectorLocation(ItemsControl itemsCtrl, Visual container, int index, bool isInput)
        {
            var item = itemsCtrl.Items[index];
            var conn = itemsCtrl.ItemContainerGenerator.ContainerFromItem(item).FindChild<Border>().FindChild<Border>();
            return conn.TransformToVisual(container).Transform(
                new Point(isInput ? 1 : conn.ActualWidth - 1, conn.ActualHeight / 2));
        }

        public Point InConnectorLocation(Visual container, int index)
        {
            return GetConnectorLocation(_icInConn, container, index, isInput: true);
        }

        public Point OutConnectorLocation(Visual container, int index)
        {
            return GetConnectorLocation(_icOutConn, container, index, isInput: false);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
        
            Mouse.SetCursor(Cursors.SizeAll);
            e.Handled = true;
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject();

                if (!ViewModel.IsSelected)
                    AreaVm.DeselectAll();

                ViewModel.IsSelected = true;

                var pointsToScreen = AreaVm.Selected.Select(nodeVm => Mouse.GetPosition(Area.NodeOfModel(nodeVm))).ToList();

                data.SetData("Objects", AreaVm.Selected);
                data.SetData("Points", pointsToScreen);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
               AreaVm.DeselectAll();
            ViewModel.IsSelected = !ViewModel.IsSelected;

            e.Handled = true;
        }

        private void UserControl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Area.SelectedNodes.ForEach(gn => gn.ViewModel.IsSelected = false);
            ViewModel.IsSelected = true;
        }
    }
}