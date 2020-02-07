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
using SmaDesktopTerminal.Windows.IndicatorsParams;
using TechAnalysisAlgLib.Indicators.MovingAverage;
using TechAnalysisAlgLib.Indicators.Oscillators;
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
        private ICommand removeIndicatorCommand;
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
                OnPropertyChanged("Name");
            }
        }

        public SeriesCollection SeriesCollectionForIndicator
        {
            get => seriesCollectionForIndicator;
            set => seriesCollectionForIndicator = value;
        }


        public ICommand OpenSettingsCommand
        {
            get => changeSettings;
            set
            {
                changeSettings = value;
                OnPropertyChanged();
            }
        }

        public ICommand RemoveIndicatorCommand
        {
            get => removeIndicatorCommand;
            set
            {
                removeIndicatorCommand = value;
                OnPropertyChanged();
            }
        }


        public void InitSeriesCollection()
        {
            if (SeriesCollectionForIndicator == null)
                SeriesCollectionForIndicator = new SeriesCollection();
            else
                SeriesCollectionForIndicator.Clear();

            switch (IndicatorShortName)
            {
                case "SMA":
                case "EMA":
                case "WMA":
                case "Momentum":
                case "AO":
                case "RSI":
                case "WPR":
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
                case "MACD":
                    {
                        var lineSeries = new GLineSeries()
                        {
                            Title = $"{Indicator.IdName} Line",
                            Fill = Brushes.Transparent,
                            Values = new GearedValues<double>()
                        };
                        var signalSeries = new GLineSeries()
                        {
                            Title = $"{Indicator.IdName} Signal",
                            Fill = Brushes.Transparent,
                            Values = new GearedValues<double>()
                        };
                        var histogramGtSeries = new GColumnSeries()
                        {
                            Title = $"{Indicator.IdName} Histogram > 0",
                            Fill = Brushes.Green,
                            Values = new GearedValues<double>()
                        };
                        var histogramLtSeries = new GColumnSeries()
                        {
                            Title = $"{Indicator.IdName} Histogram < 0",
                            Fill = Brushes.Red,
                            Values = new GearedValues<double>()
                        };
                        SeriesCollectionForIndicator.Add(lineSeries);
                        SeriesCollectionForIndicator.Add(signalSeries);
                        SeriesCollectionForIndicator.Add(histogramGtSeries);
                        SeriesCollectionForIndicator.Add(histogramLtSeries);
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
                case "MACD":
                    {

                        var closePrices = candles.Select(r => r.Close).ToList();
                        var convertedIndicator = ((Macd)Indicator).Calculate(closePrices);
                        var skipCnt = convertedIndicator.MacdLine.Count - lastLimit;
                        var subsetLine = convertedIndicator.MacdLine.Skip(skipCnt).Take(lastLimit).ToList();
                        var subsetHistogram = convertedIndicator.MacdHistogram.Skip(skipCnt).Take(lastLimit).ToList();
                        var subsetSignal = convertedIndicator.Signal.Skip(skipCnt).Take(lastLimit).ToList();



                        foreach (var oneCandle in subsetLine)
                        {
                            SeriesCollectionForIndicator?[0]?.Values.Add(oneCandle ?? double.NaN);
                        }
                        foreach (var oneCandle in subsetSignal)
                        {
                            SeriesCollectionForIndicator?[1]?.Values.Add(oneCandle ?? double.NaN);
                        }
                        foreach (var oneCandle in subsetHistogram)
                        {
                            var val = oneCandle ?? 0;
                            var valGt = val > 0 ? val : 0;
                            var valLt = val < 0 ? val : 0;
                            SeriesCollectionForIndicator?[2]?.Values.Add(valGt);
                            SeriesCollectionForIndicator?[3]?.Values.Add(valLt);
                        }

                        break;
                    }

                case "WPR":
                    {
                        var closePrices = candles.Select(r => r.Close).ToList();
                        var highPrices = candles.Select(r => r.High).ToList();
                        var lowPrices = candles.Select(r => r.Low).ToList();
                        var convertedIndicator = ((WilliamsPercentRange)Indicator).Calculate(closePrices, highPrices, lowPrices);
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

        public ChartIndicatorInfo(string indicatorShortName, Action<ChartIndicatorInfo> removeAction, Action refreshChartsAction) : this()
        {
            this.IndicatorShortName = indicatorShortName;
            this.RemoveIndicatorCommand = new RemoveIndicatorCommand(removeAction, this);
            this.OpenSettingsCommand = new OpenSettingsCommand(refreshChartsAction, this);
        }



        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class RemoveIndicatorCommand : ICommand
    {
        private readonly Action<ChartIndicatorInfo> execute;
        private readonly ChartIndicatorInfo parent;

        public RemoveIndicatorCommand(Action<ChartIndicatorInfo> action, ChartIndicatorInfo parent)
        {
            execute = action;
            this.parent = parent;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parent);
        }
    }


    public class OpenSettingsCommand : ICommand
    {
        private readonly Action refreshChartsAction;
        private readonly ChartIndicatorInfo parent;

        public OpenSettingsCommand(Action action, ChartIndicatorInfo parent)
        {
            this.parent = parent;
            this.refreshChartsAction = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {

            //var passwordWindow = new PeriodIndicatorsSettings(parent.Indicator.Period);

            //if (passwordWindow.ShowDialog() == true)
            //{
            //    //if (passwordWindow.Password == "12345678")
            //    //    //MessageBox.Show("Авторизация пройдена");
            //    //else
            //    //    //MessageBox.Show("Неверный пароль");
            //}
            //else
            //{
            //    //MessageBox.Show("Авторизация не пройдена");
            //}


            switch (parent.IndicatorShortName)
            {
                case "SMA":
                case "EMA":
                case "WMA":
                case "Momentum":
                case "RSI":
                case "WPR":
                    {
                        var periodIndicatorsSettings = new PeriodIndicatorsSettings(parent.Indicator.Period);

                        if (periodIndicatorsSettings.ShowDialog() == true)
                        {
                            if (parent.Indicator.Period == periodIndicatorsSettings.PeriodOfIndicator && periodIndicatorsSettings.PeriodOfIndicator == 0)
                                break;

                            switch (parent.IndicatorShortName)
                            {
                                case "SMA":
                                    parent.Indicator = new SimpleMovingAverage(periodIndicatorsSettings.PeriodOfIndicator);
                                    break;
                                case "EMA":
                                    parent.Indicator = new ExponentialMovingAverage(periodIndicatorsSettings.PeriodOfIndicator);
                                    break;
                                case "WMA":
                                    parent.Indicator = new WeightedMovingAverage(periodIndicatorsSettings.PeriodOfIndicator);
                                    break;
                                case "Momentum":
                                    parent.Indicator = new Momentum(periodIndicatorsSettings.PeriodOfIndicator);
                                    break;
                                //case "AO":
                                case "RSI":
                                    parent.Indicator = new RelativeStrengthIndex(periodIndicatorsSettings.PeriodOfIndicator);
                                    break;
                                case "WPR":
                                    parent.Indicator = new WilliamsPercentRange(periodIndicatorsSettings.PeriodOfIndicator);
                                    break;
                            }

                            refreshChartsAction();
                        }


                        break;
                    }
                case "MACD":
                    {
                        var macdIndicator = (Macd)parent.Indicator;
                        var periodIndicatorsSettings = new PeriodMacdIndicatorSettings(macdIndicator.Fast,
                            macdIndicator.Slow, macdIndicator.Signal);

                        if (periodIndicatorsSettings.ShowDialog() == true)
                        {
                            if (macdIndicator.Fast == periodIndicatorsSettings.Fast && macdIndicator.Slow == periodIndicatorsSettings.Slow && macdIndicator.Signal == periodIndicatorsSettings.Signal)
                                break;

                            parent.Indicator = new Macd(periodIndicatorsSettings.Fast, periodIndicatorsSettings.Slow, periodIndicatorsSettings.Signal);

                        }

                        break;
                    }
            }



        }
    }
}
