using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using SmaDesktopTerminal.Annotations;

namespace SmaDesktopTerminal.Windows.IndicatorsParams
{
    /// <summary>
    /// Логика взаимодействия для PeriodMacdIndicatorSettings.xaml
    /// </summary>
    public partial class PeriodMacdIndicatorSettings : Window, INotifyPropertyChanged
    {
        private uint fast;
        private uint slow;
        private uint signal;

        public uint Fast
        {
            get => fast;
            set
            {
                fast = value < 1 ? 1 : value;
                fast = value > 100 ? 100 : value;
                OnPropertyChanged();
            }
        }

        public uint Slow
        {
            get => slow;
            set
            {
                slow = value < 1 ? 1 : value;
                slow = value > 100 ? 100 : value;
                OnPropertyChanged();
            }
        }

        public uint Signal
        {
            get => signal;
            set
            {

                signal = value < 1 ? 1 : value;
                signal = value > 100 ? 100 : value;
                OnPropertyChanged();
            }
        }


        public PeriodMacdIndicatorSettings(uint fast, uint slow, uint signal)
        {
            DataContext = this;
            InitializeComponent();
            Fast = fast;
            Slow = slow;
            Signal = signal;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
