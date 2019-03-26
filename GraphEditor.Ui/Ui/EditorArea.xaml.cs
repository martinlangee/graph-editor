using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphEditor.Components;
using GraphEditor.Converter;
using GraphEditor.Tools;
using GraphEditor.ViewModel;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für EditorArea.xaml
    /// </summary>
    public partial class EditorArea : UserControl
    {
        Point _lineContextMenuOrigin;
        int _draggingBendPoint;

        public EditorArea()
        {
            InitializeComponent();

            DataContext = new AreaViewModel();

            MessageHub.Inst.Dispatcher = Application.Current.Dispatcher;
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

            var mousePos = Mouse.GetPosition(_canvas);
            Canvas.SetLeft(graphNode, mousePos.X);
            Canvas.SetTop(graphNode, mousePos.Y);
        }

        private void OnRemoveNode(NodeViewModel nodeVm)
        {
            var toRemove = GraphNodes.FirstOrDefault(gn => gn.DataContext.Equals(nodeVm));
            _canvas.Children.Remove(toRemove);
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
                var connVm = (ConnectionViewModel)line.DataContext;
                if (nodeVm.Equals(connVm.SourceNode))
                {
                    connVm.SetPoint(0, node.OutConnectorLocation(_canvas, connVm.SourceConnector));
                }
                if (nodeVm.Equals(connVm.TargetNode))
                {
                    connVm.SetPoint(connVm.Points.Count - 1, node.InConnectorLocation(_canvas, connVm.TargetConnector));
                }
            }
        }

        private void OnAddConnection(ConnectionViewModel connectionVm)
        {
            var line = new ArrowPolyline
            {
                SnapsToDevicePixels = true,
                Stroke = Brushes.DimGray,
                StrokeThickness = 1.2,
                HoverStrokeThickness = 2.0,
                HoverStroke = Brushes.DarkBlue,
                HeadWidth = 8,
                HeadHeight = 2,
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

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _draggingBendPoint = ((ConnectionViewModel) ((ArrowPolyline) sender).DataContext).NearestBendPointIndex(Mouse.GetPosition(_canvas));
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingBendPoint >= 0)
            {
                ((ConnectionViewModel) ((ArrowPolyline) sender).DataContext).SetPoint(_draggingBendPoint, Mouse.GetPosition(_canvas));
                Mouse.Capture((ArrowPolyline) sender);
            }
        }

        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
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
            line.ContextMenu.Items.Add(new Separator());
            line.ContextMenu.Items.Add(new MenuItem());
            line.ContextMenu.Opened += (s, e) => _lineContextMenuOrigin = Mouse.GetPosition(_canvas);

            var item = (MenuItem) line.ContextMenu.Items[0];
            item.DataContext = line.DataContext;
            item.Header = "Bend line here";
            item.Click += BendLineClick;

            item = (MenuItem) line.ContextMenu.Items[2];
            item.DataContext = line.DataContext;
            item.Header = "Delete";
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

        private void LineDeleteClick(object sender, RoutedEventArgs e)
        {
            var connVm = (ConnectionViewModel) ((FrameworkElement) sender).DataContext;
            connVm.SourceNode.RemoveConnection(connVm);
        }

        private void BendLineClick(object sender, RoutedEventArgs e)
        {
            ((ConnectionViewModel) ((MenuItem) sender).DataContext).InsertPointNear(_lineContextMenuOrigin);
        }

        private void SetDragObjectPosition(DragEventArgs e)
        {
            // Position von allen Nodes setzen, die beim Draggen selektiert sind
            var nodeVMs = (List<NodeViewModel>) e.Data.GetData("Objects");
            var points = (List<Point>) e.Data.GetData("Points");

            for (var idx = 0; idx < nodeVMs.Count; idx++)
            {
                var point = e.GetPosition(_canvas) - points[idx];
                nodeVMs[idx].Location = new Point(point.X, point.Y);
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
        }

        private void _canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Arrow;

            var selectedNodes = ViewModel.NodeVMs.Where(nv => nv.IsSelected).ToList();
            ViewModel.RevokeConnectRequestStatus();
            ViewModel.DeselectAll();
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            MessageHub.Inst.OnAddNode += OnAddNode;
            MessageHub.Inst.OnRemoveNode += OnRemoveNode;
            MessageHub.Inst.OnNodeLocationChanged += OnNodeLocationChanged;
            MessageHub.Inst.OnAddConnection += OnAddConnection;
            MessageHub.Inst.OnRemoveConnection += OnRemoveConnection;
        }

        private void Editor_Unloaded(object sender, RoutedEventArgs e)
        {
            MessageHub.Inst.Dispose();
        }
    }
}
