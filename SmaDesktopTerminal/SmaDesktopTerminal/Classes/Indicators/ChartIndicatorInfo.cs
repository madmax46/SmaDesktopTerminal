using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ExchCommonLib.Classes;
using LiveCharts;
using LiveCharts.Geared;
using TechAnalysisAlgLib.Interfaces;
using TechAnalysisAlgLib.Series;

namespace SmaDesktopTerminal.Classes.Indicators
{
    public class ChartIndicatorInfo : INotifyPropertyChanged
    {
        //private string name;
        public readonly string IndicatorShortName;
        private IIndicator indicator;
        private ICommand changeSettings;
        private SeriesCollection seriesCollectionForIndicator;
        public event PropertyChangedEventHandler PropertyChanged;


        public string Name
        {
            get => indicator?.IdName;
            set
            {
                OnPropertyChanged();
            }
        }


        public IIndicator Indicator
        {
            get => indicator;
            set
            {
                indicator = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection SeriesCollectionForIndicator
        {
            get => seriesCollectionForIndicator;
            set => seriesCollectionForIndicator = value;
        }


        public ICommand ChangeSettings
        {
            get => changeSettings;
            set
            {
                changeSettings = value;
                OnPropertyChanged();
            }
        }



        public void InitSeriesCollection()
        {
            SeriesCollectionForIndicator.Clear();
            switch (IndicatorShortName)
            {
                case "SMA":
                case "EMA":
                case "WMA":
                case "Momentum":
                case "AO":
                case "RSI":
                    {
                        var lineSeries = new GLineSeries()
                        {
                            Title = Indicator.IdName,
                            Fill = Brushes.Transparent,
                            Values = new GearedValues<double>()
                        };
                        SeriesCollectionForIndicator.Add(lineSeries);
                        break;
                    }
            }
        }

        public void CalculateIndicatorValuesAndFillSeries(List<Candle> candles, int lastLimit)
        {

            switch (IndicatorShortName)
            {
                case "SMA":
                case "EMA":
                case "WMA":
                case "Momentum":
                case "AO":
                case "RSI":
                    {
                        var closePrices = candles.Select(r => r.Close).ToList();
                        var convertedIndicator = ((IIndicator<List<double?>>)Indicator).Calculate(closePrices);
                        var skipCnt = convertedIndicator.Count - lastLimit;
                        var subset = convertedIndicator.Skip(skipCnt).Take(lastLimit).ToList();



                        foreach (var oneCandle in subset)
                        {
                            SeriesCollectionForIndicator?.FirstOrDefault()?.Values.Add(oneCandle ?? double.NaN);
                        }
                        break;
                    }
                default: break;
            }


        }


        //public void FillChartSeries(ISeries series, int lastLimit)
        //{
        //    switch (IndicatorShortName)
        //    {
        //        case "SMA":
        //        case "EMA":
        //        case "WMA":
        //            {
        //                //var closePrices = candles.Select(r => r.Close).ToList();
        //                //var convertedIndicator = ((IIndicator<List<double?>>)Indicator).Calculate(closePrices);
        //                //return new SimpleSeries(convertedIndicator);
        //                break;
        //            }
        //        default:
        //            {
        //                break;
        //            }
        //    }

        //}

        public ChartIndicatorInfo()
        {
            SeriesCollectionForIndicator = new SeriesCollection();
        }

        public ChartIndicatorInfo(string indicatorShortName) : this()
        {
            this.IndicatorShortName = indicatorShortName;
        }



        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
