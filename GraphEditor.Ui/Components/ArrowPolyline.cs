#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
// License for the specific language governing rights and limitations under the License.
#endregion

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphEditor.Ui.Components
{
    class ArrowPolyline : Shape
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(ArrowPolyline),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register(nameof(HeadWidth), typeof(double), typeof(ArrowPolyline),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register(nameof(HeadHeight), typeof(double), typeof(ArrowPolyline),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty BendPointSizeProperty = DependencyProperty.Register(nameof(BendPointSize), typeof(double), typeof(ArrowPolyline),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            private set { SetValue(PointsProperty, value); }
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

        [TypeConverter(typeof(LengthConverter))]
        public double BendPointSize
        {
            get { return (double)GetValue(BendPointSizeProperty); }
            set { SetValue(BendPointSizeProperty, value); }
        }

        bool _isHovering;
        double _origStrokeThickness;
        Brush _origStroke;

        public ArrowPolyline()
        {
            Points = new PointCollection();

            MouseMove += ArrowLine_MouseMove;
            MouseLeave += ArrowLine_MouseLeave;
            ContextMenuOpening += ArrowLine_ContextMenuOpening;
            ContextMenuClosing += ArrowLine_ContextMenuClosing;
        }

        public double HoverStrokeThickness { get; set; }
        public Brush HoverStroke { get; set; }

        private void ArrowLine_MouseMove(object sender, MouseEventArgs e)
        {
            _isHovering = true;

            if (StrokeThickness != HoverStrokeThickness)
            {
                _origStrokeThickness = StrokeThickness;
                _origStroke = Stroke;
            }
            StrokeThickness = HoverStrokeThickness;
            Stroke = HoverStroke;
        }

        private void ArrowLine_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ContextMenu == null || !ContextMenu.IsOpen)
            {
                _isHovering = false;
                StrokeThickness = _origStrokeThickness;
                Stroke = _origStroke;
            }
        }

        private void ArrowLine_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            StrokeThickness = HoverStrokeThickness;
            Stroke = HoverStroke;
        }

        private void ArrowLine_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            _isHovering = false;
            StrokeThickness = _origStrokeThickness;
            Stroke = _origStroke;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry
                {
                    FillRule = FillRule.EvenOdd
                };

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
            if (Points == null || Points.Count < 2) return;

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

            context.BeginFigure(pt3, true, false);
            context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);

            context.BeginFigure(Points[0], true, false);
            for (int zPt = 1; zPt < Points.Count; zPt++)
            {
                context.LineTo(Points[zPt], true, true);
            }

            if (!_isHovering || Points.Count < 3 || BendPointSize.Equals(0.0)) return;

            // draw bend points if mouse is hovering
            var bendPtDelta = BendPointSize / 2;

            for (var zPt = 1; zPt < Points.Count - 1; zPt++)
            {
                var pt = Points[zPt];
                context.BeginFigure(new Point(pt.X - bendPtDelta, pt.Y - bendPtDelta), true, true);
                context.LineTo(new Point(pt.X - bendPtDelta, pt.Y + bendPtDelta), true, true);
                context.LineTo(new Point(pt.X + bendPtDelta, pt.Y + bendPtDelta), true, true);
                context.LineTo(new Point(pt.X + bendPtDelta, pt.Y - bendPtDelta), true, true);
            }
        }
    }
}
