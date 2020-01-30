using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExchCommonLib.Classes.Operations;

namespace SmaDesktopTerminal.Classes.Interface
{
    public class OperationsHistoryUserController : INotifyPropertyChanged
    {
        private ObservableCollection<MarketOperation> operations;
        private Visibility portfolioProgressBarVisibility;

        public ObservableCollection<MarketOperation> Operations
        {
            get => operations;
            set
            {
                operations = value;
                OnPropertyChanged();
            }
        }

        public Visibility PortfolioProgressBarVisibility
        {
            get => portfolioProgressBarVisibility;
            set
            {
                portfolioProgressBarVisibility = value;
                OnPropertyChanged();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;



        public ActionCommand UpdateHistoryCommand { get; set; }


        public OperationsHistoryUserController()
        {
            Operations = new ObservableCollection<MarketOperation>();
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }


    public class ActionCommand : ICommand
    {
        private readonly Action execute;

        public ActionCommand(Action action)
        {
            execute = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }
}
