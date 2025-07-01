using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Entities;

namespace WpfApp1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MakettoEntity> Makettora = new();

        public void CreateMaketto(MakettoEntity ME)
        {
            Makettora.Add(ME);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
