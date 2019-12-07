using SmaDesktopTerminal.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmaDesktopTerminal.Models
{
    public class TerminalWindowModel : INotifyPropertyChanged
    {
        private readonly AppMainModel mainModel;


        private Person person;
        private ObservableCollection<Instrument> instruments;

        public Person CurPerson
        {
            get
            {
                return person;
            }
            set
            {
                person = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<Instrument> Instruments
        {
            get
            {
                return instruments;
            }
            set
            {
                instruments = value;
                OnPropertyChanged();
            }

        }


        public TerminalWindowModel(AppMainModel mainModel)
        {
            this.mainModel = mainModel;
            CurPerson = mainModel.CurPerson;
            Instruments = new ObservableCollection<Instrument>();
            Instruments.Add(new Instrument() { Name = "asdsa", IsAllow = true });
            Instruments.Add(new Instrument() { Name = "asdsa2", IsAllow = false });




            DesktopTerminalWindow desktopTerminalWindow = new DesktopTerminalWindow(this);
            desktopTerminalWindow.Show();
        }


        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CurPerson.FullName = "asdasdsssss";

        }






        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
