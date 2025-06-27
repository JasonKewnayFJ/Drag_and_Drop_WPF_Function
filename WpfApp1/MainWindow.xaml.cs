using System.Text; // No se usa aquí, pero si te hace feliz, déjalo
using System.Windows; // Cosas base de WPF
using System.Windows.Controls; // Para usar Grid, Canvas, TextBox, etc.
using System.Windows.Data; // Para bindings (aquí no usamos, pero bueno)
using System.Windows.Documents; // Por si quieres trabajar con texto más pro
using System.Windows.Input; // Para manejar el mouse, teclado, etc.
using System.Windows.Media; // Para colores, brochas, efectos visuales
using System.Windows.Media.Imaging; // Para imágenes (no usamos ahora)
using System.Windows.Navigation; // Navegación entre páginas (tampoco usamos)
using System.Windows.Shapes; // Para usar cosas como Ellipse, Rectangle, etc.

namespace WpfApp1
{
    /// <summary>
    /// Lógica del MainWindow, o sea la ventana principal
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDragging = false; // Bandera pa' saber si estamos arrastrando algo
        UIElement? DraggingElement = null; // El elemento que estamos moviendo ahora
        private Point mouseOffset; // Diferencia entre el click y el borde del elemento

        public MainWindow()
        {
            InitializeComponent(); // Carga el XAML, sin esto todo se va al carajo
        }

        private void GrabbingElement(object sender, MouseButtonEventArgs e)
        {
            DraggingElement = (UIElement)sender; // Agarramos lo que estamos clickeando
            IsDragging = true; // Decimos: "Ey, estamos en modo arrastrar"
            mouseOffset = e.GetPosition(DraggingElement); // Guardamos de dónde se agarró
            DraggingElement.CaptureMouse(); // Pa’ que el mouse siga al objeto aunque salga
        }

        private void MovingElement(object sender, MouseEventArgs e)
        {
            if (!IsDragging) return; // Si no estamos arrastrando, pues no hacemos nada

            var canvas = VisualTreeHelper.GetParent(sender as UIElement) as Canvas; // Buscamos el canvas (ojo, esto está medio frágil)
            var position = e.GetPosition(canvas); // Posición actual del mouse en el canvas

            var grid = sender as Grid; // Cast por si acaso, aunque no se usa después
            var ellipse = grid?.Children.OfType<Ellipse>().FirstOrDefault(); // Pillamos el círculo si necesitamos hacerle algo (aquí no se hace nada)

            // Movemos el objeto a donde está el mouse, pero ajustando con el offset
            Canvas.SetLeft(DraggingElement, position.X - mouseOffset.X);
            Canvas.SetTop(DraggingElement, position.Y - mouseOffset.Y);
        }

        private void ReleasingElement(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false; // Ya no estamos arrastrando
            if (DraggingElement != null)
            {
                DraggingElement.ReleaseMouseCapture(); // Soltamos el control del mouse
            }
            DraggingElement = null; // Ya no hay ningún objeto agarrado
        }

        private void CreateNewElement(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TitleBox.Text)) return; // Si no hay texto, no hacemos ni mierda

            string Title = TitleBox.Text; // Guardamos el texto
            TitleBox.Text = string.Empty; // Limpiamos el textbox

            AddDraggablePoint(Title, 400, 400); // Creamos el punto en (400, 400) con ese texto
        }

        void AddDraggablePoint(string label, double x, double y)
        {
            var grid = new Grid(); // Contenedor para juntar el círculo y el texto
            var ellipse = new Ellipse
            {
                MinHeight = 50,
                MinWidth = 50,
                Margin = new Thickness(20), // Espacio alrededor, pa’ que respire
                Fill = Brushes.CadetBlue // Color medio turquesa
            };

            var text = new TextBlock
            {
                Text = label, // El texto que va en el círculo
                Foreground = Brushes.White, // Letra blanca pa’ que se vea
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            grid.Children.Add(ellipse); // Metemos el círculo en el grid
            grid.Children.Add(text); // Metemos el texto arriba del círculo

            // Conectamos los eventos de mouse al grid entero
            grid.MouseDown += GrabbingElement;
            grid.MouseMove += MovingElement;
            grid.MouseUp += ReleasingElement;

            // Posicionamos el grid dentro del canvas
            Canvas.SetLeft(grid, x);
            Canvas.SetTop(grid, y);

            MyCanvas.Children.Add(grid); // Y por fin lo agregamos al canvas
        }
    }
}
