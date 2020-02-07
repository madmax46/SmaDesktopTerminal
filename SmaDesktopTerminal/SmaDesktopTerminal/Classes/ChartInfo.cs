using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using SmaDesktopTerminal.Models;

namespace SmaDesktopTerminal.Classes
{
    public class ChartInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TerminalWindowModel terminalWindowModel;
        private SeriesCollection seriesCollection;
        private List<string> lables;
        private double height;
        private int minAxesVal;
        private int maxAxesVal;

        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set => seriesCollection = value;
        }

        public List<string> Lables
        {
            get => lables;
            set
            {
                lables = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => height;
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }


        public int MinAxesVal
        {
            get => minAxesVal;
            set
            {
                minAxesVal = value;
                OnPropertyChanged();
                if (terminalWindowModel?.ChartsAll?.Any() == true)
                    foreach (var chartInfo in terminalWindowModel.ChartsAll)
                    {
                        if (chartInfo != this)
                            chartInfo.UpdateMinAxesVal(value);
                    }
            }
        }

        public int MaxAxesVal
        {
            get => maxAxesVal;
            set
            {
                maxAxesVal = value;
                OnPropertyChanged();

                if (terminalWindowModel?.ChartsAll?.Any() == true)
                    foreach (var chartInfo in terminalWindowModel.ChartsAll)
                    {
                        if (chartInfo != this)
                            chartInfo.UpdateMaxAxesVal(value);
                    }
            }
        }



        public ChartInfo(TerminalWindowModel terminalWindowModel)
        {
            this.terminalWindowModel = terminalWindowModel;
            minAxesVal = 0;
            maxAxesVal = 1;
        }

        //public ChartInfo()
        //{

        //}


        public void UpdateMinAxesVal(int newInterval)
        {
            minAxesVal = newInterval;
            OnPropertyChanged("MinAxesVal");
        }

        public void UpdateMaxAxesVal(int newInterval)
        {
            maxAxesVal = newInterval;
            OnPropertyChanged("MaxAxesVal");
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
