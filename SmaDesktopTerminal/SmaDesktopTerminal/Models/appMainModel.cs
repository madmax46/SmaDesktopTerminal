using SmaDesktopTerminal.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmaDesktopTerminal.Models
{
    public class AppMainModel : INotifyPropertyChanged
    {
        private Person person;


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


        public AppMainModel()
        {

            CurPerson = new Person()
            {
                FullName = "Глушаков Максим Васильевич"
            };

            TerminalWindowModel windowModel = new TerminalWindowModel(this);
            //DesktopTerminalWindow terminalWindow = new DesktopTerminalWindow();
            //terminalWindow.Show();
            //Windows.LogInWindow logInWindow = new Windows.LogInWindow();
            //logInWindow.Show();



        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
