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
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Lógica del MainWindow, o sea la ventana principal
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDragging = false; // Bandera pa' saber si estamos arrastrando algo
        UIElement? DraggingElement { get; set; } // El elemento que estamos moviendo ahora
        UIElement? DraggedElement = null; // El elemente que estuvo movido anteriormente 
        private Point mouseOffset; // Diferencia entre el click y el borde del elemento
        private readonly List<(UIElement a, UIElement b, Line connection)> Links = new(); //La lista pa'actualizar la linea durante el movimiento del elemento
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

            //var canvas = MyCanvas; // Buscamos el canvas (ojo, esto está medio frágil)
            var position = e.GetPosition(MyCanvas); // Posición actual del mouse en el canvas

            foreach(var (a,b,line) in Links)
            {
                if (a == DraggingElement || b == DraggingElement)
                {
                    Point p1 = GetCenter(a);
                    Point p2 = GetCenter(b);

                    line.X1 = p1.X;
                    line.X2 = p2.X;
                    line.Y1 = p1.Y;
                    line.Y2 = p2.Y; 
                }
            }

            // Movemos el objeto a donde está el mouse, pero ajustando con el offset
            Canvas.SetLeft(DraggingElement, position.X - mouseOffset.X);
            Canvas.SetTop(DraggingElement, position.Y - mouseOffset.Y);
        }

        private void ReleasingElement(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false; // Ya no estamos arrastrando
            if (DraggingElement != null)
            {
                DraggedElement = DraggingElement; 
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
            var border = new Border
            {
                MinHeight = 50,
                MinWidth = 50,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(10), // Espacio alrededor, pa’ que respire
                Background = Brushes.Red // Color medio turquesa
            };

            var text = new TextBlock
            {
                Text = label, // El texto que va en el círculo
                Foreground = Brushes.White, // Letra blanca pa’ que se vea
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            grid.Children.Add(border); // Metemos el círculo en el grid
            grid.Children.Add(text); // Metemos el texto arriba del círculo

            // Conectamos los eventos de mouse al grid entero
            grid.MouseDown += GrabbingElement;
            grid.MouseMove += MovingElement;
            grid.MouseUp += ReleasingElement;
            grid.MouseRightButtonDown += Link;


            // Posicionamos el grid dentro del canvas
            Canvas.SetLeft(grid, x);
            Canvas.SetTop(grid, y);

            MyCanvas.Children.Add(grid); // Y por fin lo agregamos al canvas
        }

        private void Link(object sender, MouseButtonEventArgs e)
        {
            if (DraggingElement == (UIElement)sender)
            {
                MessageBox.Show("Нельзя связать элемент с самим собой");
                DraggingElement = null;
                return;
            }
            if (DraggingElement == null) DraggingElement = (UIElement)sender;
            else
            {
                DraggedElement = (UIElement)sender;
            }
          

            if (DraggingElement != null && DraggedElement != null)
            {
                LinkFromTo(DraggingElement, DraggedElement);
                DraggedElement = null;
                DraggingElement = null;
            }


        }

        private void LinkFromTo(UIElement firstElement, UIElement secondElement)
        {
            Point start = GetCenter(firstElement);
            Point end = GetCenter(secondElement);

            var line = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,

                Stroke = Brushes.Black,
                StrokeThickness = 2,
                IsHitTestVisible = false
            };

            MyCanvas.Children.Add(line);
            Links.Add((firstElement, secondElement, line));
        }

        Point GetCenter(UIElement element)
        {
            double x = Canvas.GetLeft(element);
            double y = Canvas.GetTop(element);
            if (element is FrameworkElement fe)
            {
                y += fe.ActualHeight / 2;
                x += fe.ActualWidth / 2;
            }
            return new Point(x, y);
        }
    }
}
