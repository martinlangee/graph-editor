using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphEditor.Components;
using GraphEditor.ViewModel;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für EditorArea.xaml
    /// </summary>
    public partial class EditorArea : UserControl
    {
        Path _connLine;

        public EditorArea()
        {
            InitializeComponent();

            DataContext = new AreaViewModel(OnAddNode, OnRemoveNode, OnNodeLocationChanged, OnAddConnection, OnUpdateConnections, OnRemoveConnection);
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
            var node = NodeOfModel(nodeVm);
            Canvas.SetLeft(node, location.X);
            Canvas.SetTop(node, location.Y);
        }

        private void CreateLineContextMenu(ArrowPolyline line)
        {
            line.ContextMenu = new ContextMenu();
            line.ContextMenu.Items.Add(new MenuItem());
            line.ContextMenu.Items.Add(new Separator());
            line.ContextMenu.Items.Add(new MenuItem());

            var item = (MenuItem)line.ContextMenu.Items[0];
            item.DataContext = line.DataContext;
            item.Header = "Bend line here";
            item.Click += LineBendClick;

            item = (MenuItem)line.ContextMenu.Items[2];
            item.DataContext = line.DataContext;
            item.Header = "Delete";
            item.Click += LineDeleteClick;
        }

        private void LineDeleteClick(object sender, RoutedEventArgs e)
        {
            var connVm = (ConnectionViewModel)((FrameworkElement)sender).DataContext;
            connVm.SourceNode.RemoveConnection(connVm);
        }

        private void LineBendClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnAddConnection(ConnectionViewModel connectionVm)
        {
            var line = new ArrowPolyline
            {
                Stroke = Brushes.DimGray,
                StrokeThickness = 1.3,
                HoverStrokeThickness = 3,
                HeadWidth = 8,
                HeadHeight = 2,
                BendPointSize = 6,
                DataContext = connectionVm
            };

            var p1 = NodeOfModel(connectionVm.SourceNode).OutConnectorLocation(_canvas, connectionVm.SourceConnector);
            var p2 = NodeOfModel(connectionVm.TargetNode).InConnectorLocation(_canvas, connectionVm.TargetConnector);

            line.Points.Add(p1);
            //line.Points.Add(new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2));  // only test
            line.Points.Add(p2);

            CreateLineContextMenu(line);

            _canvas.Children.Add(line);
        }

        private void OnUpdateConnections(NodeViewModel nodeVm)
        {
            var node = NodeOfModel(nodeVm);
            
            foreach (var line in ConnectionLines)
            {
                var connVm = (ConnectionViewModel) line.DataContext;
                if (nodeVm.Equals(connVm.SourceNode))
                {
                    line.Points[0] = node.OutConnectorLocation(_canvas, connVm.SourceConnector);
                }
                if (nodeVm.Equals(connVm.TargetNode))
                {
                    line.Points[line.Points.Count - 1] = node.InConnectorLocation(_canvas, connVm.TargetConnector);
                }
            }
        }

        private void OnRemoveConnection(ConnectionViewModel connectionVm)
        {
            _canvas.Children.Remove(LineOfModel(connectionVm));
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

            // workaround to place the connections correctly after node dropped
            Thread.Sleep(10);
            var nodeVMs = (List<NodeViewModel>)e.Data.GetData("Objects");
            nodeVMs.ForEach(nv => OnUpdateConnections(nv));
        }

        private void _canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.DeselectAll();
        }
    }
}
