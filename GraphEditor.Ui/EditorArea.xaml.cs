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
using System.ComponentModel;


/* ----------------------------------------------------------------------------------
 * TODO:
 *  Form des Nodes anpassbar machen (Kreis, Halbkreis etc.)
 *  Verhalten der Linien-Komponente verbessern (evtl. selektierbar?)
 *  Zoom
 *  Drucken
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

        internal List<NodeViewModel> SelectedNodesVms => SelectedNodes.Select(gn => gn.NodeVm).ToList();

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
            var mainWindow = this.FindAncestor<Window>();

            if (mainWindow != null)
                mainWindow.Closing += Editor_Closing;

            UiMessageHub.OnAddNode += OnAddNode;
            UiMessageHub.OnRemoveNode += OnRemoveNode;
            UiMessageHub.OnNodeLocationChanged += OnNodeLocationChanged;
            UiMessageHub.OnAddConnection += OnAddConnection;
            UiMessageHub.OnRemoveConnection += OnRemoveConnection;
            UiMessageHub.OnSetZIndex += OnSetZIndex;
        }

        private void Editor_Closing(object sender, CancelEventArgs e)
        {
            UiMessageHub.Dispose();
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            var selected = SelectedNodesVms;

            selected.ForEach(nodeVm =>
            {
                if (e.Key == Key.F4 && selected.Count == 1)
                {
                    nodeVm.EditConfigExec();
                }
                else
                if (e.Key == Key.Delete)
                {
                    nodeVm.RemoveExec();
                }
                else
                {
                    var offsetPoint = nodeVm.Location;

                    if (e.Key == Key.Left) offsetPoint.Offset(-10, 0);
                    if (e.Key == Key.Up) offsetPoint.Offset(0, -10);
                    if (e.Key == Key.Right) offsetPoint.Offset(10, 0);
                    if (e.Key == Key.Down) offsetPoint.Offset(0, 10);

                    nodeVm.Location = offsetPoint;
                }
            });
        }
    }
}
