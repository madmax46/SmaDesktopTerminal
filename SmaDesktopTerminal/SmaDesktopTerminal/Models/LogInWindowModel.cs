using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AuthCommonLib;
using AuthCommonLib.Authenticate;
using ExchCommonLib.Rest;
using SmaDesktopTerminal.Classes.Interface;
using SmaDesktopTerminal.Windows;

namespace SmaDesktopTerminal.Models
{
    public class LogInWindowModel : INotifyPropertyChanged
    {
        private readonly string authServiceUrl;
        private readonly HttpClient httpClientService;
        private readonly AppMainModel mainModel;
        private readonly LogInWindow logInWindow;
        private string login;
        private string password;
        private ICommand logInClickCommand;
        private Visibility errorVisibility;
        public event PropertyChangedEventHandler PropertyChanged;



        public string Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }


        public ICommand LogInClickCommand
        {
            get => logInClickCommand;
            set
            {
                logInClickCommand = value;
                OnPropertyChanged();
            }
        }

        public Visibility ErrorVisibility
        {
            get => errorVisibility;
            set
            {
                errorVisibility = value;
                OnPropertyChanged();
            }
        }

        public LogInWindowModel(AppMainModel mainModel, string authServiceUrl)
        {
            this.mainModel = mainModel;
            this.authServiceUrl = authServiceUrl;
            this.LogInClickCommand = new ActionCommand(LogInTask);
            httpClientService = new HttpClient();
            ErrorVisibility = Visibility.Hidden;
            logInWindow = new LogInWindow(this);
            logInWindow.DataContext = this;
            logInWindow.Show();
        }



        public async void LogInTask()
        {
            try
            {
                var hashPassword = PassUtils.GetMd5PassHash(logInWindow.PasswordBox.Password);
                var request = new AuthenticateRequest()
                {
                    Login = this.Login,
                    Password = hashPassword
                };

                var res = await CallRest.PostAsync<AuthenticateRequest, AuthenticateResponse>(
                    this.authServiceUrl, request, httpClientService);

                if (res?.Response == null || !res.Response.IsSuccess)
                {
                    ErrorVisibility = Visibility.Visible;
                    return;
                }

                ErrorVisibility = Visibility.Hidden;
                logInWindow.Hide();
                mainModel.SuccessLogin(res.Response);

            }
            catch (Exception ex)
            {
                ErrorVisibility = Visibility.Visible;

            }
            finally
            {
            }

        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ShutDownApp()
        {
            mainModel.ShutDownApp();
        }
    }



}
