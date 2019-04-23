#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
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
