﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphEditor.Components
{
    class ArrowPolyline : Shape
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(ArrowPolyline), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", typeof(double), typeof(ArrowPolyline), 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", typeof(double), typeof(ArrowPolyline), 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        double _origStrokeThickness;

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadWidth
        {
            get { return (double)GetValue(HeadWidthProperty); }
            set { SetValue(HeadWidthProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadHeight
        {
            get { return (double)GetValue(HeadHeightProperty); }
            set { SetValue(HeadHeightProperty, value); }
        }

        public ArrowPolyline()
        {
            Points = new PointCollection();
            MouseMove += ArrowLineMouseMove;
            MouseLeave += ArrowLineMouseLeave;
            ContextMenuOpening += ArrowLineContextMenuOpening;
            ContextMenuClosing += ArrowLineContextMenuClosing;
        }

        public double HoverStrokeThickness { get; set; }

        private void ArrowLineMouseMove(object sender, MouseEventArgs e)
        {
            if (StrokeThickness != HoverStrokeThickness)
                _origStrokeThickness = StrokeThickness;
            StrokeThickness = HoverStrokeThickness;
        }

        private void ArrowLineMouseLeave(object sender, MouseEventArgs e)
        {
            if (ContextMenu == null || !ContextMenu.IsOpen)
                StrokeThickness = _origStrokeThickness;
        }

        private void ArrowLineContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            StrokeThickness = HoverStrokeThickness;
        }

        private void ArrowLineContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            StrokeThickness = _origStrokeThickness;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    InternalDrawArrowGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            if (Points.Count < 2) return;

            var pt1 = Points[Points.Count - 2];
            var pt2 = Points[Points.Count - 1];

            double theta = Math.Atan2(pt1.Y - pt2.Y, pt1.X - pt2.X);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pt3 = new Point(
                pt2.X + (HeadWidth * cost - HeadHeight * sint),
                pt2.Y + (HeadWidth * sint + HeadHeight * cost));

            Point pt4 = new Point(
                pt2.X + (HeadWidth * cost + HeadHeight * sint),
                pt2.Y - (HeadHeight * cost - HeadWidth * sint));

            context.BeginFigure(pt1, true, false);
            foreach (var pt in Points)
            {
                context.LineTo(pt, true, true);
            }
            context.LineTo(pt3, true, true);
            context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);
        }
    }
}
