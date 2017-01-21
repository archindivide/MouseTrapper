using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WpfScreenHelper;

namespace MouseTrapper.Helpers
{
    static class CaptureHelper
    {
        private static List<Screen> _monitors;

        private static Screen _currentMonitor;

        private static RECT _defaultCapArea;
        //private static Point _prevMousePos;

        private static bool HasChanged { get; set; }

        private static bool _isMultMon;
        private static bool IsMultMon
        {
            get
            {
                if (HasChanged)
                {
                    bool one = false;
                    _isMultMon = false;
                    foreach (var screen in _monitors)
                    {
                        Rect bounds = new Rect(new Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1), new Point(screen.Bounds.Right - 1, screen.Bounds.Bottom - 1));
                        if (bounds.IntersectsWith(BoundArea))
                        {
                            if (one)
                            {
                                _isMultMon = true;
                                return _isMultMon;
                            }
                            one = true;
                            _currentMonitor = screen;
                            HasChanged = false;
                        }
                    }
                    return _isMultMon;
                }
                else
                {
                    return _isMultMon;
                }
            }
        }

        private static Point _startPosition;
        public static Point StartPosition
        {
            get
            {
                return _startPosition;
            }
            set
            {
                _startPosition = value;
                HasChanged = true;
            }
        }

        private static Point _endPosition;
        public static Point EndPosition
        {
            get
            {
                return _endPosition;
            }
            set
            {
                _endPosition = value;
                HasChanged = true;
            }
        }

        private static Rect _boundArea;
        public static Rect BoundArea
        {
            get
            {
                if (_boundArea == null || HasChanged)
                {
                    _boundArea = new Rect(StartPosition, EndPosition);
                }
                return _boundArea;
            }
        }

        public static string CoordinateDisplay
        {
            get
            {
                StringBuilder coorDisp = new StringBuilder();

                if (IsMultMon)
                {
                    coorDisp.Append("Multi");
                }
                else
                {
                    coorDisp.Append($"Mon{_monitors.IndexOf(_currentMonitor) + 1}");
                }

                coorDisp.Append("://");
                coorDisp.Append($"({StartPosition.X},{StartPosition.Y}),({EndPosition.X},{EndPosition.Y})");

                return coorDisp.ToString();
            }
        }

        public static bool IsCapturing { get; set; }

        public static void Initialize(IntPtr handle)
        {
            _monitors = Screen.AllScreens.ToList();
            _currentMonitor = Screen.FromHandle(handle);

            GetClipCursor(out _defaultCapArea);

            StartPosition = _currentMonitor.Bounds.TopLeft;
            EndPosition = _currentMonitor.Bounds.BottomRight;
        }

        public static void CaptureMouse(bool reset)
        {
            if (reset)
            {
                ClipCursor(ref _defaultCapArea);
            }
            else
            {
                RECT clip;
                GetClipCursor(out clip);

                RECT area = (RECT)BoundArea;

                if (clip != area)
                {
                    ClipCursor(ref area);
                }
            }
        }

        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //public static extern int SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ClipCursor(ref RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetClipCursor(out RECT lpRect);

        public struct RECT
        {
            #region Variables.
            /// <summary>
            /// Left position of the rectangle.
            /// </summary>
            public int Left;
            /// <summary>
            /// Top position of the rectangle.
            /// </summary>
            public int Top;
            /// <summary>
            /// Right position of the rectangle.
            /// </summary>
            public int Right;
            /// <summary>
            /// Bottom position of the rectangle.
            /// </summary>
            public int Bottom;
            #endregion

            #region Operators.
            /// <summary>
            /// Operator to convert a RECT to Drawing.Rectangle.
            /// </summary>
            /// <param name="rect">Rectangle to convert.</param>
            /// <returns>A Drawing.Rectangle</returns>
            public static implicit operator Rect(RECT rect)
            {
                return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }

            /// <summary>
            /// Operator to convert Drawing.Rectangle to a RECT.
            /// </summary>
            /// <param name="rect">Rectangle to convert.</param>
            /// <returns>RECT rectangle.</returns>
            public static implicit operator RECT(Rect rect)
            {
                return new RECT((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
            }

            public static bool operator ==(RECT first, RECT second)
            {
                return first.Top == second.Top && 
                       first.Bottom == second.Bottom &&
                       first.Left == second.Left &&
                       first.Right == second.Right;
            }

            public static bool operator !=(RECT first, RECT second)
            {
                return first.Top != second.Top ||
                       first.Bottom != second.Bottom ||
                       first.Left != second.Left ||
                       first.Right != second.Right;
            }
            #endregion

            #region Constructor.
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="left">Horizontal position.</param>
            /// <param name="top">Vertical position.</param>
            /// <param name="right">Right most side.</param>
            /// <param name="bottom">Bottom most side.</param>
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
            #endregion
        }
    }
}

//Code Dumpster

//public static void CaptureMouse(Object source, System.Timers.ElapsedEventArgs e)
//{
//    var mousePos = WpfScreenHelper.MouseHelper.MousePosition;

//    if (!BoundArea.Contains(mousePos))
//    {

//        //bool useY = false;
//        //bool useX = false;
//        //if (mousePos.Y < BoundArea.Top || mousePos.Y > BoundArea.Top + BoundArea.Height)
//        //{
//        //    useY = true;
//        //}
//        //if (mousePos.X < BoundArea.Left || mousePos.X > BoundArea.Left + BoundArea.Width)
//        //{
//        //    useX = true;
//        //}

//        var rectCenterX = BoundArea.Left + BoundArea.Width / 2;
//        var rectCenterY = BoundArea.Top + BoundArea.Height / 2;

//        //var pointYDist = Math.Abs(rectCenterY - mousePos.Y);
//        //var pointXDist = Math.Abs(rectCenterX - mousePos.X);
//        //var pointDist = Math.Sqrt(Math.Pow((pointYDist), 2) + Math.Pow((pointXDist), 2));

//        //var rectAngle = Math.Asin(pointXDist / pointDist);

//        //double rectAdjLength = 0;

//        //if (useX)
//        //{
//        //    rectAdjLength = Math.Abs(rectCenterX - BoundArea.Left);
//        //}
//        //else
//        //{
//        //    rectAdjLength = Math.Abs(rectCenterY - BoundArea.Top);
//        //}

//        //var rectOpposLength = rectAdjLength / Math.Tan(rectAngle);

//        //int pointX = 0;
//        //int pointY = 0;

//        //if(rectXDist > rectCenterX)
//        //{
//        //    pointX = (int)Math.Floor(rectXDist);
//        //}
//        //else
//        //{
//        //    pointX = (int)Math.Ceiling(rectXDist);
//        //}

//        //if(rectYDist > rectCenterY)
//        //{
//        //    pointY = (int)Math.Floor(rectYDist);
//        //}
//        //else
//        //{
//        //    pointY = (int)Math.Ceiling(rectYDist);
//        //}

//        SetCursorPos((int)rectCenterX, (int)rectCenterY);
//    }
//    else
//    {
//        SetCursorPos((int)_prevMousePos.X, (int)_prevMousePos.Y);
//    }
//}

//public static void CaptureMouse(bool reset)
//{
//    if(reset)
//    {
//        ClipCursor(_defaultCapArea);
//    }
//    else
//    {
//        ClipCursor(new RECT((long) StartPosition.X,
//                            (long) StartPosition.Y,
//                            (long) EndPosition.X,
//                            (long) EndPosition.Y));
//    }
//}

//[System.Runtime.InteropServices.DllImport("user32.dll")]
//public static extern bool GetClipCursor(out RECT lpRect);

//[System.Runtime.InteropServices.DllImport("user32.dll")]
//public static extern bool ClipCursor(RECT lpRect);

//public struct RECT
//{
//    long left;
//    long top;
//    long right;
//    long bottom;

//    public RECT(long l, long t, long r, long b)
//    {
//        left = l;
//        top = t;
//        right = r;
//        bottom = b;
//    }
//}

//public static void CaptureMouse(Object source, System.Timers.ElapsedEventArgs e)
//{
//    var mousePos = WpfScreenHelper.MouseHelper.MousePosition;

//    if (!BoundArea.Contains(mousePos))
//    {
//        SetCursorPos((int)_prevMousePos.X, (int)_prevMousePos.Y);
//    }
//    else
//    {
//        _prevMousePos = mousePos;
//    }
//}

//public static void StartCapture()
//{
//    while(IsCapturing)
//    {
//        var mousePos = WpfScreenHelper.MouseHelper.MousePosition;

//        if (!BoundArea.Contains(mousePos))
//        {
//            SetCursorPos((int)_prevMousePos.X, (int)_prevMousePos.Y);
//        }
//        else
//        {
//            _prevMousePos = mousePos;
//        }
//    }     
//}