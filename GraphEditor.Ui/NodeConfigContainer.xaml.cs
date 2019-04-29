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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphEditor.Ui
{
    /// <summary>
    /// Interaction logic for NodeConfigContainer.xaml
    /// </summary>
    public partial class NodeConfigContainer : UserControl
    {
        bool _dragging;
        Point _mouseStartPoint;
        Point _dragStartPoint;

        public NodeConfigContainer()
        {
            InitializeComponent();
        }

        private void TbHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragging = true;
            _mouseStartPoint = Mouse.GetPosition(Parent as IInputElement);
            _dragStartPoint = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            Mouse.Capture(TbHeader);
        }

        private void TbHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = Mouse.GetPosition(Parent as IInputElement);
                Canvas.SetLeft(this, _dragStartPoint.X + mousePos.X - _mouseStartPoint.X);
                Canvas.SetTop(this, _dragStartPoint.Y + mousePos.Y - _mouseStartPoint.Y);
            }
        }

        private void TbHeader_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragging = false;
            Mouse.Capture(null);
        }
    }
}
