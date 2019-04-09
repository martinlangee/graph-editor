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


/* ----------------------------------------------------------------------------------
 * TODO:
 *  Erzeugung über Drag-n-Drop aus der Toolbar
 *  z-Order der Nodes editierbar machen
 *  wer-kann-an-wen beispielhaft implementieren
 *  Connectoren können "State" haben (z.B. durch Farbe visualisiert)
 * 
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

            DataContext = ServiceContainer.Get<AreaViewModel>();

            UiMessageHub.Dispatcher = Application.Current.Dispatcher;
        }

        private AreaViewModel ViewModel => (AreaViewModel) DataContext;

        private List<GraphNode> GraphNodes => _canvas.Children.OfType<GraphNode>().ToList();

        internal List<GraphNode> SelectedNodes => GraphNodes.Where(gn => gn.ViewModel.IsSelected).ToList();

        internal GraphNode NodeOfModel(NodeViewModel viewModel) => GraphNodes.FirstOrDefault(gn => gn.ViewModel.Equals(viewModel));

        private List<ArrowPolyline> ConnectionLines => _canvas.Children.OfType<ArrowPolyline>().ToList();

        private ArrowPolyline LineOfModel(ConnectionViewModel viewModel) => ConnectionLines.FirstOrDefault(cp => cp.DataContext.Equals(viewModel));

        private void OnAddNode(NodeViewModel nodeVm)
        {
            var graphNode = new GraphNode { DataContext = nodeVm };

            _canvas.Children.Add(graphNode);

            var location = Mouse.GetPosition(_canvas);

            if (ViewModel.ToolBar.IsGridShown)
            {
                var gridWidth = ViewModel.GridRect.Width;
                location.X = Math.Round(location.X / gridWidth) * gridWidth;
                location.Y = Math.Round(location.Y / gridWidth) * gridWidth;
            }

            nodeVm.Location = location;
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
                    connVm.MovePoint(0, node.OutConnectorLocation(_canvas, connVm.SourceConnector));
                }
                if (nodeVm.Equals(connVm.TargetNode))
                {
                    connVm.MovePoint(connVm.Points.Count - 1, node.InConnectorLocation(_canvas, connVm.TargetConnector));
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
            CreateLinePointsBinding(line);

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
                var location = Mouse.GetPosition(_canvas);

                if (ViewModel.ToolBar.IsGridShown)
                {
                    var gridWidth = ViewModel.GridRect.Width;
                    location.X = Math.Round(location.X / gridWidth) * gridWidth;
                    location.Y = Math.Round(location.Y / gridWidth) * gridWidth;
                }

                ConnectionVmFromLine(sender).MovePoint(_draggingBendPoint, location);
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

        private void CreateLinePointsBinding(ArrowPolyline line)
        {
            Binding pointsBinding = new Binding
            {
                Source = line.DataContext,
                Path = new PropertyPath(nameof(line.Points)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                Converter = new ListToPointCollectionConverter()
            };
            BindingOperations.SetBinding(line, ArrowPolyline.PointsProperty, pointsBinding);
        }

        private MenuItem FindMenuItemByTag(object tag, ContextMenu contextMenu)
        {
            foreach (var item in contextMenu.Items.Cast<MenuItem>())
            {
                if (tag.Equals(item.Tag))
                    return item;
            }
            return null;
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
            // Position von allen Nodes setzen, die beim Draggen selektiert sind
            var nodeVMs = (List<NodeViewModel>) e.Data.GetData(UiConst.DragDropObjects);
            var points = (List<Point>) e.Data.GetData(UiConst.DragDropPoints);

            for (var idx = 0; idx < nodeVMs.Count; idx++)
            {
                var locVector = e.GetPosition(_canvas) - points[idx];
                var location = new Point(locVector.X, locVector.Y);

                if (ViewModel.ToolBar.IsGridShown)
                {
                    var gridWidth = ViewModel.GridRect.Width;
                    location.X = Math.Round(location.X / gridWidth) * gridWidth;
                    location.Y = Math.Round(location.Y / gridWidth) * gridWidth;
                }
                
                nodeVMs[idx].Location = location;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            SetDragObjectPosition(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            SetDragObjectPosition(e);
            Mouse.Capture(null);
        }

        private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Arrow;

            var selectedNodes = ViewModel.NodeVMs.Where(nv => nv.IsSelected).ToList();
            ViewModel.RevokeConnectRequestStatus();
            ViewModel.DeselectAll();
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            UiMessageHub.OnAddNode += OnAddNode;
            UiMessageHub.OnRemoveNode += OnRemoveNode;
            UiMessageHub.OnNodeLocationChanged += OnNodeLocationChanged;
            UiMessageHub.OnAddConnection += OnAddConnection;
            UiMessageHub.OnRemoveConnection += OnRemoveConnection;
        }

        private void Editor_Unloaded(object sender, RoutedEventArgs e)
        {
            UiMessageHub.Dispose();
        }
    }
}
