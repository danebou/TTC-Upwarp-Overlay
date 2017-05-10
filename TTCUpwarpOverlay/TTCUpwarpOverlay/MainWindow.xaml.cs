using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace TTCUpwarpOverlay {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        string ImageDir = Environment.CurrentDirectory + @"\video\";

        int _curImage = 0;
        int _maxFile = 0;

        bool _sliderProgrammically = false;

        double _lastLeft, _lastTop;
        double _imageMaxWidth, _imageMaxHeight;

        public MainWindow() {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            _maxFile = 350;

            _lastLeft = Left;
            _lastTop = Top;
            _imageMaxWidth = ImageDisplay.Width;
            _imageMaxHeight = ImageDisplay.Height;
        }

        private void UpdateImage() {
            ImageDisplay.Source = new BitmapImage(new Uri(String.Format("{0}{1:0000}.png", ImageDir, _curImage)));
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e) {
            if (_curImage <= 1)
                _curImage = _maxFile;
            else
                _curImage--;

            UpdateImage();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e) {
            if (_curImage >= _maxFile)
                _curImage = 1;
            else
                _curImage++;

            UpdateImage();
        }

        private void frameDrag_MouseDown(object sender, MouseButtonEventArgs e) {
            if (sliderXAxis.Value != 50 || sliderYAxis.Value != 50) {
                _sliderProgrammically = true;
                sliderXAxis.Value = 50;
                sliderYAxis.Value = 50;
                _sliderProgrammically = false;
            }

            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

            _lastLeft = Left;
            _lastTop = Top;
        }

        private void sliderTransparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ImageDisplay.Opacity = (100 - e.NewValue) / 100;
        }

        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd) {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            //SetWindowExTransparent(hwnd);
        }

        private void sliderXAxis_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_sliderProgrammically)
                return;

            this.Left = _lastLeft + (sliderXAxis.Value - 50) / 4;
        }

        private void sliderYAxis_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_sliderProgrammically)
                return;

            this.Top = _lastTop + (sliderYAxis.Value - 50) / 4; 
        }

        private void sliderXScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_imageMaxWidth == 0)
                return;
            ImageDisplay.Width = _imageMaxWidth - sliderXScale.Value; 
        }

        private void sliderYScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_imageMaxHeight == 0)
                return;
            ImageDisplay.Height = _imageMaxHeight - sliderYScale.Value; 
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (ImageDisplay.Visibility == Visibility.Visible) {
                ImageDisplay.Visibility = Visibility.Collapsed;
            }
            else {
                ImageDisplay.Visibility = Visibility.Visible;
            }
        }
    }
}
