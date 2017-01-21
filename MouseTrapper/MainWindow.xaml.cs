using MouseTrapper.Helpers;
using Ownskit.Utils;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace MouseTrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _timer;
        private KeyboardListener _keyboardListener;
        private RawKeyEventHandler _keyEventHandler;

        public MainWindow()
        {
            InitializeComponent();
            CaptureHelper.Initialize(new System.Windows.Interop.WindowInteropHelper(this).Handle);

            lblVersion.Content = $"Version {typeof(MainWindow).Assembly.GetName().Version}";
            txtCoordinates.Text = CaptureHelper.CoordinateDisplay;

            _keyboardListener = new KeyboardListener();
            _keyEventHandler = new RawKeyEventHandler(KeyboardListener_KeyDown);

            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = 100;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CaptureHelper.CaptureMouse(!CaptureHelper.IsCapturing);
        }

        private void btnRemapCoordinates_Click(object sender, RoutedEventArgs e)
        {
            CaptureWindow window = new CaptureWindow();
            window.Show();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            CaptureHelper.IsCapturing = !CaptureHelper.IsCapturing;

            if (CaptureHelper.IsCapturing)
            {
                btnRemapCoordinates.IsEnabled = false;
                txtCoordinates.IsReadOnly = true;
                btnStartStop.Content = "Stop";

                _timer.Start();

                _keyboardListener.KeyDown += _keyEventHandler;
            }
            else
            {
                btnRemapCoordinates.IsEnabled = true;
                txtCoordinates.IsReadOnly = false;
                btnStartStop.Content = "Start";

                _timer.Stop();
                //After stopping the timer we need to reset the clip
                CaptureHelper.CaptureMouse(true);

                _keyboardListener.KeyDown -= _keyEventHandler; 
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtCoordinates.Text = CaptureHelper.CoordinateDisplay;
            lblError.Content = "";
        }

        private void KeyboardListener_KeyDown(object sender, RawKeyEventArgs e)
        {
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if(e.Key == Key.F7)
                {
                    btnStartStop_Click(null, null);
                }
            }
        }

        private void txtCoordinates_KeyDown(object sender, KeyEventArgs e)
        {
            lblError.Content = "";
            if(e.Key == Key.Enter)
            {
                ParseCoordinates(txtCoordinates.Text);
            }
        }

        private void txtCoordinates_LostFocus(object sender, RoutedEventArgs e)
        {
            lblError.Content = "";
            ParseCoordinates(txtCoordinates.Text);
        }

        private void ParseCoordinates(string coord)
        {
            try
            {
                //Form: Mon2://(0,0),(1920,1080)

                string[] split = coord.Split('/');
                string coordPart = split[split.Length-1];

                split = coordPart.Split(',');

                string startX = split[0].Replace("(", "");
                string startY = split[1].Replace(")", "");

                string endX = split[2].Replace("(", "");
                string endY = split[3].Replace(")", "");

                CaptureHelper.StartPosition = new Point(int.Parse(startX), int.Parse(startY));
                CaptureHelper.EndPosition = new Point(int.Parse(endX), int.Parse(endY)); 
            }
            catch
            {
                lblError.Content = "Invalid Coordinates.\nForm: #,#,#,# or like displayed.";
            }
            finally
            {
                txtCoordinates.Text = CaptureHelper.CoordinateDisplay;
            }
        }
    }
}
