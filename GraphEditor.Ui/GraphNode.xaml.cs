#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Interface.Container;
using GraphEditor.Interface.Ui;
using GraphEditor.Ui.Tools;
using GraphEditor.Ui.ViewModel;


namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für GraphNode.xaml
    /// </summary>
    public partial class GraphNode : UserControl
    {
        private bool _dragging;
        private readonly Func<NodeViewModel, GraphNode> _onGetNodeOfModel;

        public GraphNode(Func<NodeViewModel, GraphNode> onGetNodeOfModel)
        {
            InitializeComponent();

            _onGetNodeOfModel = onGetNodeOfModel;
        }

        internal NodeViewModel NodeVm => DataContext as NodeViewModel;
         
        internal IAreaViewModel AreaVm => ServiceContainer.Get<IAreaViewModel>();

        private Point GetConnectorLocation(ItemsControl itemsCtrl, Visual container, int index, bool isOutBound)
        {
            if (itemsCtrl.Items.Count == 0) return new Point(0, 0);

            var item = itemsCtrl.Items[index];
            var conn = itemsCtrl.ItemContainerGenerator.ContainerFromItem(item).FindChild<Border>().FindChild<Border>();
            return conn.TransformToVisual(container).Transform(
                new Point(isOutBound ? conn.ActualWidth: -1, conn.ActualHeight / 2));
        }

        public Point InConnectorLocation(Visual container, int index)
        {
            return GetConnectorLocation(_icInConn, container, index, isOutBound: false);
        }

        public Point OutConnectorLocation(Visual container, int index)
        {
            return GetConnectorLocation(_icOutConn, container, index, isOutBound: true);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
        
            Mouse.SetCursor(Cursors.SizeAll);
            e.Handled = true;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragging = true;
            AreaVm.RevokeConnectRequestStatus();

            if (e.ClickCount > 1)
            {
                NodeVm.EditConfigCommand.Execute(sender);
            }
        }

        private void Border_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (!NodeVm.IsSelected)
                    AreaVm.DeselectAll();

                NodeVm.IsSelected = true;

                AreaVm.RevokeConnectRequestStatus();

                var pointsToScreen = AreaVm.Selected.Cast<NodeViewModel>().Select(nodeVm => Mouse.GetPosition(_onGetNodeOfModel(nodeVm))).ToList();

                var data = new DataObject();
                data.SetData(UiConst.DragDropData_Nodes, AreaVm.Selected);
                data.SetData(UiConst.DragDropData_Points, pointsToScreen);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        private void Border_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragging = false;

            if (Keyboard.Modifiers != ModifierKeys.Control)
               AreaVm.DeselectAll();
            NodeVm.IsSelected = !NodeVm.IsSelected;

            e.Handled = true;
        }

        private void GraphNode_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            AreaVm.DeselectAll();
            NodeVm.IsSelected = true;
        }
    }
}