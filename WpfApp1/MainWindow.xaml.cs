using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDragging = false;
        UIElement DraggingElement;
        private Point mouseOffset;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GrabbingElement(object sender, MouseButtonEventArgs e)
        {
            DraggingElement = (UIElement)sender;
            IsDragging = true;
            mouseOffset = e.GetPosition(DraggingElement);
            DraggingElement.CaptureMouse();
        }

        private void MovingElement(object sender, MouseEventArgs e)
        {
            if (!IsDragging) return;
            var canvas = VisualTreeHelper.GetParent(sender as UIElement) as Canvas;
            var position = e.GetPosition(canvas);
            Canvas.SetLeft(DraggingElement, position.X - mouseOffset.X);
            Canvas.SetTop(DraggingElement, position.Y - mouseOffset.Y);
        }

        private void ReleasingElement(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
            DraggingElement.ReleaseMouseCapture();
        }
    }
}