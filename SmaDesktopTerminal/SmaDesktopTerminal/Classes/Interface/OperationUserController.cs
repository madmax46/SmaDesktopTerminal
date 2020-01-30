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
using ExchCommonLib.Classes.Exchange;

namespace SmaDesktopTerminal.Classes.Interface
{
    public class OperationUserController : INotifyPropertyChanged
    {
        //private ObservableCollection<PortfolioItem> positions;

        private uint count;

        private double operationPrice;
        private ObservableCollection<Instrument> instruments;
        private Instrument selectedInstrument;
        private DateTime date;

        //private PortfolioItem selectedPortfolioItem;

        public event PropertyChangedEventHandler PropertyChanged;


        public double OperationPrice
        {
            get => operationPrice;
            set
            {
                operationPrice = value;
                OnPropertyChanged();
            }
        }

        public uint Count
        {
            get => count;
            set
            {
                count = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged();
            }
        }


        public BuyOperationToClick BuyOperationToClick { get; set; }
        public SellOperationToClick SellOperationToClick { get; set; }

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


        public Instrument SelectedInstrument
        {
            get => selectedInstrument;
            set
            {
                selectedInstrument = value;
                OnPropertyChanged();
            }
        }


        public OperationUserController(ObservableCollection<Instrument> instruments)
        {
            Instruments = instruments;
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

    public class BuyOperationToClick : ICommand
    {
        private Action execute;

        public BuyOperationToClick(Action action)
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

    public class SellOperationToClick : ICommand
    {
        private Action execute;


        public event EventHandler CanExecuteChanged;


        public SellOperationToClick(Action action)
        {
            execute = action;
        }


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
