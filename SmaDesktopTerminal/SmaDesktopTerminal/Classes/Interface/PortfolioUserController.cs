using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExchCommonLib.Classes.Exchange;
using ExchCommonLib.Classes.UserPortfolio;

namespace SmaDesktopTerminal.Classes.Interface
{
    public class PortfolioUserController : INotifyPropertyChanged
    {
        private ObservableCollection<PortfolioItem> positions;
        private double totalAmount;
        private PortfolioItem selectedPortfolioItem;
        private Visibility portfolioProgressBarVisibility;

        public event PropertyChangedEventHandler PropertyChanged;

        public double TotalAmount
        {
            get => totalAmount;
            set
            {
                totalAmount = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PortfolioItem> Positions
        {
            get => positions;
            set
            {
                positions = value;
                OnPropertyChanged();
            }
        }

        public PortfolioItem SelectedPortfolioItem
        {
            get => selectedPortfolioItem;
            set
            {
                selectedPortfolioItem = value;
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



        public PortfolioUserController()
        {
            positions = new ObservableCollection<PortfolioItem>();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
