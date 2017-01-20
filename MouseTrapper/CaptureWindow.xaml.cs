using MouseTrapper.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MouseTrapper
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private bool _mouseDown;
        private Point _mouseDownPos;
        public CaptureWindow()
        {
            InitializeComponent();
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Cursor = Cursors.Cross;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDownPos = e.GetPosition(this);
            CaptureHelper.StartPosition = WpfScreenHelper.MouseHelper.MousePosition;
            _mouseDown = true;
            rectSelection.Visibility = Visibility.Visible;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CaptureHelper.EndPosition = WpfScreenHelper.MouseHelper.MousePosition;
            this.Close();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                // When the mouse is held down, reposition the drag selection box.

                Point mousePos = e.GetPosition(this);

                if (_mouseDownPos.X < mousePos.X)
                {
                    Canvas.SetLeft(rectSelection, _mouseDownPos.X);
                    rectSelection.Width = mousePos.X - _mouseDownPos.X;
                }
                else
                {
                    Canvas.SetLeft(rectSelection, mousePos.X);
                    rectSelection.Width = _mouseDownPos.X - mousePos.X;
                }

                if (_mouseDownPos.Y < mousePos.Y)
                {
                    Canvas.SetTop(rectSelection, _mouseDownPos.Y);
                    rectSelection.Height = mousePos.Y - _mouseDownPos.Y;
                }
                else
                {
                    Canvas.SetTop(rectSelection, mousePos.Y);
                    rectSelection.Height = _mouseDownPos.Y - mousePos.Y;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
