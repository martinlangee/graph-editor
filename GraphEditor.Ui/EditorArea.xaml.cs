#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Ui.Components;
using GraphEditor.Ui.Converters;
using GraphEditor.Interface.Container;
using GraphEditor.Ui.Tools;
using GraphEditor.Ui.ViewModel;
using System;
using GraphEditor.Interface.Utils;
using GraphEditor.Interface.Nodes;
using GraphEditor.Interface.Ui;


/* ----------------------------------------------------------------------------------
 * TODO:
 *  Verhalten der Linien-Komponente verbessern (evtl. selektierbar?)
 * ------------------------------------------------------------------------------- */


namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für EditorArea.xaml
    /// </summary>
    public partial class EditorArea : UserControl
    {
        Point _lineContextMenuOrigin;
        int _draggingBendPoint = -1;

        const int TagAddBendPoint = 100;
        const int TagRemoveBendPoint = 200;

        public EditorArea()
        {
            InitializeComponent();

            DataContext = ServiceContainer.Get<IAreaViewModel>() as AreaViewModel;

            UiMessageHub.Dispatcher = Application.Current.Dispatcher;
        }

        private AreaViewModel AreaVm => (AreaViewModel) DataContext;

        private List<GraphNode> GraphNodes => _canvas.Children.OfType<GraphNode>().ToList();

        internal List<GraphNode> SelectedNodes => GraphNodes.Where(gn => gn.NodeVm.IsSelected).ToList();

        internal GraphNode NodeOfModel(NodeViewModel viewModel) => GraphNodes.FirstOrDefault(gn => gn.NodeVm.Equals(viewModel));

        private List<ArrowPolyline> ConnectionLines => _canvas.Children.OfType<ArrowPolyline>().ToList();

        private ArrowPolyline LineOfModel(ConnectionViewModel viewModel) => ConnectionLines.FirstOrDefault(cp => cp.DataContext.Equals(viewModel));

        private Point GetGridSnappedLocation(Point location)
        {
            if (AreaVm.ToolBar.IsGridVisible)
            {
                var gridWidth = AreaVm.GridRect.Width;
                location.X = Math.Round(location.X / gridWidth) * gridWidth;
                location.Y = Math.Round(location.Y / gridWidth) * gridWidth;
            }
            return location;
        }

        private void OnSetZIndex(NodeViewModel nodeVm, bool up)
        {
            var node = NodeOfModel(nodeVm);

            var allNodes = _canvas.Children.OfType<GraphNode>().ToList();

            var nodeIdx = allNodes.IndexOf(node);

            var startIdx = up ? nodeIdx + 1 : 0;
            var stopIdx = up ? allNodes.Count() - 1 : nodeIdx - 1;

            for (var z = startIdx; z <= stopIdx; z++)
            {
                Canvas.SetZIndex(allNodes[z], Canvas.GetZIndex(allNodes[z]) + (up ? -1 : 1));
            }

            Canvas.SetZIndex(node, up ? allNodes.Count() - 1 : 0);

            _canvas.Children.OfType<ArrowPolyline>().For((line, i) => Canvas.SetZIndex(line, allNodes.Count() + i));
        }

        private void OnAddNode(NodeViewModel nodeVm, Point location)
        {
            var graphNode = new GraphNode(nVm => NodeOfModel(nVm)) { DataContext = nodeVm };

            _canvas.Children.Add(graphNode);

            nodeVm.Location = GetGridSnappedLocation(location.X < 0 ? Mouse.GetPosition(_canvas) : location);
        }

        private void OnRemoveNode(NodeViewModel nodeVm)
        {
            var nodeToRemove = GraphNodes.FirstOrDefault(gn => gn.DataContext.Equals(nodeVm));
            _canvas.Children.Remove(nodeToRemove);
        }

        private void OnNodeLocationChanged(NodeViewModel nodeVm, Point location)
        {
            // set new node location
            var node = NodeOfModel(nodeVm);

            if (node == null) return;

            Canvas.SetLeft(node, location.X);
            Canvas.SetTop(node, location.Y);

            // update concerned connection lines

            foreach (var line in ConnectionLines)
            {
                var connVm = (ConnectionViewModel) line.DataContext;

                if (nodeVm.Equals(connVm.SourceNode))
                {
                    var newLocation = node.OutConnectorLocation(_canvas, connVm.SourceConnector);

                    if (connVm.LastPointIndex > 1 &&
                        Math.Round(connVm.Points[1].Y).Equals(Math.Round(connVm.Points[0].Y)))
                    {
                        connVm.MovePoint(1, newLocation.Y);
                    }

                    connVm.MovePoint(0, newLocation);
                }

                if (nodeVm.Equals(connVm.TargetNode))
                {
                    var newLocation = node.InConnectorLocation(_canvas, connVm.TargetConnector);

                    if (connVm.LastPointIndex > 1 && 
                        Math.Round(connVm.Points[connVm.LastPointIndex - 1].Y).Equals(Math.Round(connVm.Points[connVm.LastPointIndex].Y)))
                    {
                        connVm.MovePoint(connVm.LastPointIndex - 1, newLocation.Y);
                    }

                    connVm.MovePoint(connVm.LastPointIndex, newLocation);
                }
            }
        }

        private void OnAddConnection(ConnectionViewModel connectionVm)
        {
            var line = new ArrowPolyline
            {
                Stroke = Brushes.DimGray,
                StrokeThickness = 1.2,
                HoverStrokeThickness = 2.0,
                HoverStroke = Brushes.DarkBlue,
                HeadWidth = 7,
                HeadHeight = 1.5,
                BendPointSize = 8,
                DataContext = connectionVm,
            };

            line.MouseLeftButtonDown += Line_MouseLeftButtonDown;
            line.MouseMove += Line_MouseMove;
            line.MouseLeftButtonUp += Line_MouseLeftButtonUp;

            CreateConnectionContextMenu(line);
            CreateLineBindings(line);

            var p1 = NodeOfModel(connectionVm.SourceNode).OutConnectorLocation(_canvas, connectionVm.SourceConnector);
            var p2 = NodeOfModel(connectionVm.TargetNode).InConnectorLocation(_canvas, connectionVm.TargetConnector);

            connectionVm.AddPoint(p1);
            connectionVm.AddPoint(p2);

            _canvas.Children.Add(line);
        }

        private ConnectionViewModel ConnectionVmFromMenuItem(object sender)
        {
            return (ConnectionViewModel) ((MenuItem) sender).DataContext;
        }

        private ConnectionViewModel ConnectionVmFromLine(object sender)
        {
            return (ConnectionViewModel) ((ArrowPolyline) sender).DataContext;
        }

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _draggingBendPoint = ConnectionVmFromLine(sender).NearestBendPointIndex(Mouse.GetPosition(_canvas));
            if (_draggingBendPoint >= 0)
            {
                Mouse.Capture((ArrowPolyline) sender);
            }
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingBendPoint >= 0)
            {
                ConnectionVmFromLine(sender).MovePoint(_draggingBendPoint, GetGridSnappedLocation(Mouse.GetPosition(_canvas)));
            }
        }

        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggingBendPoint >= 0)
            {
                Mouse.Capture(null);
            }
            _draggingBendPoint = -1;
        }

        private void OnRemoveConnection(ConnectionViewModel connectionVm)
        {
            _canvas.Children.Remove(LineOfModel(connectionVm));
        }

        private void CreateConnectionContextMenu(ArrowPolyline line)
        {
            line.ContextMenu = new ContextMenu();
            line.ContextMenu.Items.Add(new MenuItem());
            line.ContextMenu.Items.Add(new MenuItem());
            line.ContextMenu.Items.Add(new Separator());
            line.ContextMenu.Items.Add(new MenuItem());
            line.ContextMenu.Tag = line;
            line.ContextMenu.Opened += LineMenu_ContextMenuOpening;

            var item = (MenuItem) line.ContextMenu.Items[1];
            item.DataContext = line.DataContext;
            item.Header = "Bend connection here";
            item.Tag = TagAddBendPoint;
            item.Click += AddBendPointClick;

            item = (MenuItem) line.ContextMenu.Items[0];
            item.DataContext = line.DataContext;
            item.Header = "Straighten connection here";
            item.Tag = TagRemoveBendPoint;
            item.Click += RemoveBendPointClick; ;

            item = (MenuItem) line.ContextMenu.Items[3];
            item.DataContext = line.DataContext;
            item.Header = "Delete connection";
            item.Tag = 0;
            item.Click += LineDeleteClick;
        }

        private void CreateLineBindings(ArrowPolyline line)
        {
            // binding between connection view model points and line point list
            Binding pointsBinding = new Binding
            {
                Source = line.DataContext,  // = ConnectionViewModel
                Path = new PropertyPath(nameof(ConnectionViewModel.Points)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                Converter = new ListToPointCollectionConverter()
            };
            BindingOperations.SetBinding(line, ArrowPolyline.PointsProperty, pointsBinding);

            // binding between connection view model state and the line stroke
            Binding strokeBinding = new Binding
            {
                Source = line.DataContext,  // = ConnectionViewModel
                Path = new PropertyPath(nameof(ConnectionViewModel.State)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
            };
            BindingOperations.SetBinding(line, ArrowPolyline.StrokeProperty, strokeBinding);
        }

        private MenuItem FindMenuItemByTag(object tag, ContextMenu contextMenu)
        {
            return contextMenu.Items.Cast<MenuItem>().FirstOrDefault(item => tag.Equals(item.Tag));
        }

        private void LineMenu_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            _lineContextMenuOrigin = Mouse.GetPosition(_canvas);

            var contextMenu = (ContextMenu) sender;
            var isNearBendPoint = ConnectionVmFromLine(contextMenu.Tag).NearestBendPointIndex(Mouse.GetPosition(_canvas)) > -1;

            FindMenuItemByTag(TagAddBendPoint, contextMenu).Visibility = isNearBendPoint ? Visibility.Collapsed : Visibility.Visible;
            FindMenuItemByTag(TagRemoveBendPoint, contextMenu).Visibility = isNearBendPoint ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddBendPointClick(object sender, RoutedEventArgs e)
        {
            ConnectionVmFromMenuItem(sender).InsertPointNear(_lineContextMenuOrigin);
        }

        private void RemoveBendPointClick(object sender, RoutedEventArgs e)
        {
            ConnectionVmFromMenuItem(sender).RemovePointNear(_lineContextMenuOrigin);
        }

        private void LineDeleteClick(object sender, RoutedEventArgs e)
        {
            var connVm = (ConnectionViewModel) ((FrameworkElement) sender).DataContext;
            connVm.SourceNode.RemoveConnection(connVm);
        }

        private void SetDragObjectPosition(DragEventArgs e)
        {
            // set positions of all selected nodes while dragging
            var nodeVMs = (e.Data.GetData(UiConst.DragDropData_Nodes) as IList<INodeViewModel>).Cast<NodeViewModel>().ToList();
            var points = e.Data.GetData(UiConst.DragDropData_Points) as List<Point>;

            for (var idx = 0; idx < nodeVMs.Count; idx++)
            {
                var locVector = e.GetPosition(_canvas) - points[idx];
                nodeVMs[idx].Location = GetGridSnappedLocation(new Point(locVector.X, locVector.Y));
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (e.Data.GetData(UiConst.DragDropData_Nodes) != null)
                SetDragObjectPosition(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            var typeData = (INodeTypeData) e.Data.GetData(UiConst.DragDropData_NodeType);

            if (typeData != null)
            {
                AreaVm.AddNodeExec(typeData, e.GetPosition(_canvas));
            }
            else
                SetDragObjectPosition(e);

            Mouse.Capture(null);
        }

        private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AreaVm.RevokeConnectRequestStatus();
            AreaVm.DeselectAll();
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            UiMessageHub.OnAddNode += OnAddNode;
            UiMessageHub.OnRemoveNode += OnRemoveNode;
            UiMessageHub.OnNodeLocationChanged += OnNodeLocationChanged;
            UiMessageHub.OnAddConnection += OnAddConnection;
            UiMessageHub.OnRemoveConnection += OnRemoveConnection;
            UiMessageHub.OnSetZIndex += OnSetZIndex;
        }

        private void Editor_Unloaded(object sender, RoutedEventArgs e)
        {
            UiMessageHub.Dispose();
        }
    }
}
