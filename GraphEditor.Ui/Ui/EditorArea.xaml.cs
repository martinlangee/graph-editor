﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphEditor.ViewModel;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaktionslogik für EditorArea.xaml
    /// </summary>
    public partial class EditorArea : UserControl
    {
        Path _drawLine;
        const double LineThickness = 1.3;
        const double LineThicknessHovered = 3;

        public EditorArea()
        {
            InitializeComponent();

            DataContext = new EditorAreaViewModel(OnAddGraphNode, OnRemoveGraphNode);
        }

        private EditorAreaViewModel ViewModel => (EditorAreaViewModel) DataContext;

        internal GraphNode NodeOfModel(GraphNodeViewModel viewModel) => GraphNodes.FirstOrDefault(gn => gn.ViewModel.Equals(viewModel));

        private List<GraphNode> GraphNodes => _canvas.Children.OfType<GraphNode>().ToList();

        internal List<GraphNode> SelectedNodes => GraphNodes.Where(gn => gn.ViewModel.IsSelected).ToList();

        private void OnAddGraphNode(GraphNodeViewModel graphNodeVm)
        {
            var graphNode = new GraphNode { DataContext = graphNodeVm };

            _canvas.Children.Add(graphNode);

            var mousePos = Mouse.GetPosition(_canvas);
            Canvas.SetLeft(graphNode, mousePos.X);
            Canvas.SetTop(graphNode, mousePos.Y);
        }

        private void OnRemoveGraphNode(GraphNodeViewModel graphNodeVm)
        {
            var toRemove = _canvas.Children.OfType<GraphNode>().FirstOrDefault(gn => gn.DataContext.Equals(graphNodeVm));
            _canvas.Children.Remove(toRemove);
        }

        private void SetDragObjectPosition(DragEventArgs e)
        {
            // Position von allen Nodes setzen, die beim Draggen selektiert sind
            var nodeVMs = (List<GraphNodeViewModel>) e.Data.GetData("Objects");
            var points = (List<Point>) e.Data.GetData("Points");

            for (var idx = 0; idx < nodeVMs.Count; idx++)
            {
                var point = e.GetPosition(_canvas) - points[idx];
                var node = NodeOfModel(nodeVMs[idx]);
                Canvas.SetLeft(node, point.X);
                Canvas.SetTop(node, point.Y);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            SetDragObjectPosition(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            SetDragObjectPosition(e);
        }

        private void _canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _drawLine = null;

            ViewModel.DeselectAll();
        }

        private void _canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_drawLine != null) return;
            if (sender is Border) return;
            if (!Equals(((FrameworkElement) e.OriginalSource).Tag, "OutConnector")) return;

            _drawLine = new Path
            {
                Data = new PathGeometry
                (
                    new List<PathFigure>
                    {
                        new PathFigure
                        (
                            e.GetPosition(_canvas),
                            new List<LineSegment>{new LineSegment(e.GetPosition(_canvas), true)},
                            false
                        )
                    }
                )
            };
            _drawLine.Stroke = Brushes.DimGray;
            _drawLine.StrokeThickness = LineThickness;
            _drawLine.MouseDown += Line_MouseDown;
            _drawLine.MouseMove += Line_MouseMove;
            _drawLine.MouseLeave += Line_MouseLeave;
            //_drawLine.DataContext = 
            _canvas.Children.Add(_drawLine);
         }

        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            ((Path) sender).StrokeThickness = LineThicknessHovered;
        }

        private void Line_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Path) sender).StrokeThickness = LineThickness;
        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawLine == null) return;
            
            ((LineSegment)((PathGeometry)_drawLine.Data).Figures[0].Segments[0]).Point = e.GetPosition(_canvas);

            e.Handled = true;            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
