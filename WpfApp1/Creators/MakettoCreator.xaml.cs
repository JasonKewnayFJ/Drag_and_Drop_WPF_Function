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
using System.Windows.Shapes;
using WpfApp1.Entities;

namespace WpfApp1.Creators
{
    /// <summary>
    /// Логика взаимодействия для MakettoCreator.xaml
    /// </summary>
    public partial class MakettoCreator : Window
    {
        MainWindow MW = (MainWindow)Application.Current.MainWindow;
        public MakettoCreator()
        {
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var Maketto = new MakettoEntity
            {
                Id = Guid.NewGuid().ToString(),
                Title = TitleBox.Text,
                Description = DescriptionBox.Text
            };

            MW.AddToCanvas(Maketto);
            this.Close();
        }
    }
}
