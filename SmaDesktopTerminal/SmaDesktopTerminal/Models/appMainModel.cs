using AuthCommonLib.Authenticate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AuthCommonLib;
using Person = SmaDesktopTerminal.Classes.Person;

namespace SmaDesktopTerminal.Models
{
    public class AppMainModel : INotifyPropertyChanged
    {

        private readonly string urlLogIn = "http://89.208.196.51:7777/login";

        private TerminalWindowModel terminalWindowModel;

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

        private LogInWindowModel logInWindowModel;

        public event PropertyChangedEventHandler PropertyChanged;


        public AppMainModel()
        {


#if DEBUG

            SuccessLogin(new AuthenticateResponse()
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWFkbWF4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJVc2VySWQiOiIxIiwiRmlyc3ROYW1lIjoi0JzQsNC60YHQuNC8IiwiU2Vjb25kTmFtZSI6ItCT0LvRg9GI0LDQutC-0LIiLCJFbWFpbCI6ImdsdXNoYWtvdm1heEBtYWlsLnJ1IiwiaXNzIjoiU21hQXV0aFNlcnZpY2UiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0LyJ9.tGKQu2877h_8axW8LtOmCpZ_DZRThEsXyNmj269BS9g",
                User = new UserInfo()
                {
                    FirstName = "Максим",
                    SecondName = "Глушаков"
                }
            });

#else 
            ShowLogInWindow();
#endif


            //CurPerson = new Person()
            //{
            //    FullName = "Глушаков Максим Васильевич"
            //};

            //TerminalWindowModel WindowModel = new TerminalWindowModel(this);
            //DesktopTerminalWindow terminalWindow = new DesktopTerminalWindow();
            //terminalWindow.Show();
            //Windows.LogInWindow logInWindow = new Windows.LogInWindow();
            //logInWindow.Show();



        }

        private void ShowLogInWindow()
        {
            logInWindowModel = new LogInWindowModel(this, urlLogIn);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void SuccessLogin(AuthenticateResponse response)
        {

            var person = new Person()
            {
                FullName = $"{response.User.SecondName} {response.User.FirstName}",
                AuthToken = response.Token
            };
            CurPerson = person;
            terminalWindowModel = new TerminalWindowModel(this, CurPerson);
        }


        internal void TerminalLogOut()
        {
            terminalWindowModel = null;
            CurPerson = null;
            ShowLogInWindow();
        }

        internal void ShutDownApp()
        {
            System.Windows.Application.Current.Shutdown();
        }

    }
}
