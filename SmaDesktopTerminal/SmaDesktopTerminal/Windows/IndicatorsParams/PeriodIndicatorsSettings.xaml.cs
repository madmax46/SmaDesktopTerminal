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
    /// Логика взаимодействия для MovingAverageParams.xaml
    /// </summary>
    public partial class PeriodIndicatorsSettings : Window, INotifyPropertyChanged
    {
        private uint periodOfIndicator;

        public PeriodIndicatorsSettings(uint currentPeriodOfIndicator)
        {

            DataContext = this;
            InitializeComponent();
            PeriodOfIndicator = currentPeriodOfIndicator;
        }

        public uint PeriodOfIndicator
        {
            get => periodOfIndicator;
            set
            {
                periodOfIndicator = value < 1 ? 1 : value;
                periodOfIndicator = value > 100 ? 100 : value;
                OnPropertyChanged();
            }
        }


        private void MaTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
