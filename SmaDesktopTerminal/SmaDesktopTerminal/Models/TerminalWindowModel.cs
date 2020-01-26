using ExchCommonLib.Classes.Exchange;
using ExchCommonLib.Classes.Requests;
using ExchCommonLib.Classes.Responses;
using ExchCommonLib.Rest;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SmaDesktopTerminal.Classes;
using SmaDesktopTerminal.Classes.ServiceResponses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ExchCommonLib.Classes;
using ExchCommonLib.Enums;
using SmaDesktopTerminal.Classes.Analytics;
using ColumnSeries = LiveCharts.Wpf.ColumnSeries;

namespace SmaDesktopTerminal.Models
{
    public class TerminalWindowModel : INotifyPropertyChanged
    {
        private readonly AppMainModel mainModel;

        private readonly string urlToService;
        private readonly string authToken;
        private readonly HttpClient httpClientService;
        private readonly DesktopTerminalWindow desktopTerminalWindow;

        public event PropertyChangedEventHandler PropertyChanged;

        private PlotModel candlesChart;
        private ObservableCollection<Instrument> instruments;

        private Person person;
        private Instrument selectedInstrument;
        private string selectedChartInterval;
        private List<string> chartIntervals;
        private SeriesCollection seriesCollection;
        private SeriesCollection seriesVolumeCollection;
        private List<string> seriesLabels;
        private InstrumentsAnalysisInfo movingAveragesInfo;
        private InstrumentsAnalysisInfo oscillatorsInfo;
        private InstrumentsAnalysisInfo mlAnalysisInfo;
        private int minAxesVal = 0;
        private int maxAxesVal = 1;

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
            get
            {
                return selectedInstrument;
            }
            set
            {
                selectedInstrument = value;
                OnPropertyChanged();
            }

        }
        public List<string> ChartIntervals
        {
            get
            {
                return chartIntervals;
            }
            set
            {
                chartIntervals = value;
                OnPropertyChanged();
            }

        }
        public string SelectedChartInterval
        {
            get
            {
                return selectedChartInterval;
            }
            set
            {
                selectedChartInterval = value;
                OnPropertyChanged();
            }

        }

        public PlotModel CandlesChart
        {
            get
            {
                return candlesChart;
            }
            set
            {
                candlesChart = value;
                OnPropertyChanged();
            }

        }


        public SeriesCollection SeriesCollection
        {
            get
            {
                return seriesCollection;
            }
            set
            {
                seriesCollection = value;
                OnPropertyChanged();
            }

        }
        public List<string> Labels
        {
            get
            {
                return seriesLabels;
            }
            set
            {
                seriesLabels = value;
                OnPropertyChanged();
            }

        }



        public SeriesCollection SeriesVolumeCollection
        {
            get
            {
                return seriesVolumeCollection;
            }
            set
            {
                seriesVolumeCollection = value;
                OnPropertyChanged();
            }

        }


        public InstrumentsAnalysisInfo MovingAveragesInfo
        {
            get => movingAveragesInfo;
            set
            {
                movingAveragesInfo = value;
                OnPropertyChanged();
            }
        }

        public InstrumentsAnalysisInfo OscillatorsInfo
        {
            get => oscillatorsInfo;
            set
            {
                oscillatorsInfo = value;
                OnPropertyChanged();
            }
        }

        public InstrumentsAnalysisInfo MlAnalysisInfo
        {
            get => mlAnalysisInfo;
            set
            {
                mlAnalysisInfo = value;
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
            }
        }

        public int MaxAxesVal
        {
            get => maxAxesVal;
            set
            {
                maxAxesVal = value;
                OnPropertyChanged();
            }
        }

        public TerminalWindowModel(AppMainModel mainModel)
        {
            this.mainModel = mainModel;
            authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWFkbWF4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJpc3MiOiJTbWFBdXRoU2VydmljZSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3QvIn0.h5yBoP5P0GFuR-E2pv4fF-A1FQXcIy2HB_dvJ1vycA8";
            urlToService = "http://localhost:4000/";

            httpClientService = new HttpClient();
            httpClientService.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            CurPerson = mainModel.CurPerson;
            Instruments = new ObservableCollection<Instrument>();
            CandlesChart = new PlotModel();

            SeriesCollection = new SeriesCollection();
            SeriesVolumeCollection = new SeriesCollection();
            Labels = new List<string>();

            desktopTerminalWindow = new DesktopTerminalWindow(this);
            desktopTerminalWindow.Show();

            InitIntervals();
            //ReloadCandles();
        }

        private void InitIntervals()
        {
            ChartIntervals = new List<string>()
            {
                "1min",
                "5min",
                "15min",
                "30min",
                "hour",
                "day",
                "week",
                "month"
            };
        }

        public async void ReloadParsedInstrumentsAsync()
        {
            try
            {
                desktopTerminalWindow.instrumentsProgressBar.Visibility = System.Windows.Visibility.Visible;
                instruments.Clear();
                var res = await CallRest.GetAsync<InstrumentsResponse>($"{urlToService}api/v1/market/GetParsedInstruments", httpClientService);
                //Thread.Sleep(10000);
                if (res?.Response?.Instruments?.Any() == true)
                {
                    res.Response.Instruments.ForEach(r => Instruments.Add(r));
                }

                desktopTerminalWindow.instrumentsProgressBar.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {

            }
        }



        public void ReloadCandles()
        {
            try
            {
                VolumeStyle volumeStyle = VolumeStyle.Combined;
                var series = new CandleStickAndVolumeSeries
                {
                    PositiveColor = OxyColors.DarkGreen,
                    NegativeColor = OxyColors.Red,
                    PositiveHollow = false,
                    NegativeHollow = false,
                    SeparatorColor = OxyColors.Gray,
                    SeparatorLineStyle = LineStyle.Dash,
                    VolumeStyle = volumeStyle,
                    //StrokeThickness = 3
                };


                series.Append(new OhlcvItem(1, 3, 5, 1, 4));
                series.Append(new OhlcvItem(2, 4, 5, 1, 3));
                Random random = new Random();
                for (int i = 3; i < 20; i++)
                {
                    var item = random.Next(1, i);
                    var item2 = random.Next(1, i);
                    var item3 = random.Next(1, i);
                    var item4 = random.Next(1, i);

                    series.Append(new OhlcvItem(i, item, item3, item4, item2));
                }

                //desktopTerminalWindow.oxyPlotView.InvalidatePlot(true);
                PlotModel plotModel = new PlotModel();
                //CandlesChart.Series.Add(series);
                plotModel.Series.Add(series);
                CandlesChart = plotModel;

            }
            catch (Exception ex)
            {

            }
        }


        public void InstrumentSelectionChanged()
        {
            ReloadCandlesForChart();
            ReloadInstrumentAnalysis();
        }

        private async void ReloadCandlesForChart()
        {
            try
            {
                try
                {
                    if (SelectedInstrument == null)
                    {
                        return;
                    }


                    desktopTerminalWindow.chartProgressBar.Visibility = System.Windows.Visibility.Visible;
                    CandlesRequest candlesRequest = new CandlesRequest()
                    {
                        InstrumentId = (uint)SelectedInstrument.FinamEmitentIDInt,
                        Interval = selectedChartInterval,
                        DateStart = new DateTime(2019, 1, 1),
                        DateEnd = new DateTime(2020, 1, 1)
                    };
                    var res = await CallRest.PostAsync<CandlesRequest, CandlesResponse>($"{urlToService}api/v1/market/candles",
                        candlesRequest, httpClientService);

                    if (res?.Response?.Candles?.Any() == true)
                    {
                        FillPlotByNewData(res.Response);
                    }

                    desktopTerminalWindow.chartProgressBar.Visibility = System.Windows.Visibility.Hidden;
                }
                catch (Exception ex)
                {
                }


                ///api/v{version}/market/candles
            }
            catch (Exception ex)
            {
            }
        }

        private async void ReloadInstrumentAnalysis()
        {
            try
            {
                try
                {
                    if (SelectedInstrument == null)
                    {
                        return;
                    }


                    //desktopTerminalWindow.chartProgressBar.Visibility = System.Windows.Visibility.Visible;
                    var predictionRequest = new PredictionRequest()
                    {
                        InstrumentId = (uint)SelectedInstrument.FinamEmitentIDInt,
                        Interval = selectedChartInterval,
                    };

                    var res = await CallRest.PostAsync<PredictionRequest, PredictionResponse>($"{urlToService}api/v1/market/analytics/GetPredictionFor",
                        predictionRequest, httpClientService);
                    res.Response.Predictions.ForEach(r => r.PredictionDecision = TranslatePredictionDecision(r.PredictionDecision));

                    var grouped = res.Response.Predictions.GroupBy(r => r.IndicatorType);
                    var dict = grouped
                        .ToDictionary(r => r.Key, r => r.ToList());


                    dict.TryGetValue(1, out var movingAvg);
                    MovingAveragesInfo = new InstrumentsAnalysisInfo(movingAvg);

                    dict.TryGetValue(2, out var oscillator);
                    OscillatorsInfo = new InstrumentsAnalysisInfo(oscillator);

                    dict.TryGetValue(3, out var mlMethods);
                    MlAnalysisInfo = new InstrumentsAnalysisInfo(mlMethods);


                    //if (res?.Response?.Candles?.Any() == true)
                    //{
                    //    FillPlotByNewData(res.Response);
                    //}

                    desktopTerminalWindow.chartProgressBar.Visibility = System.Windows.Visibility.Hidden;
                }
                catch (Exception ex)
                {
                }


                ///api/v{version}/market/candles
            }
            catch (Exception ex)
            {
            }
        }



        //private void FillPlotByNewData(CandlesResponse candlesResponse)
        //{
        //    VolumeStyle volumeStyle = VolumeStyle.Stacked;
        //    var series = new CandleStickAndVolumeSeries
        //    {
        //        PositiveColor = OxyColors.Green,
        //        NegativeColor = OxyColors.Red,
        //        PositiveHollow = false,
        //        NegativeHollow = false,
        //        SeparatorColor = OxyColors.Gray,
        //        SeparatorLineStyle = LineStyle.Dash,
        //        VolumeStyle = volumeStyle,
        //        //StrokeThickness = 3
        //    };
        //    //CandlesChart.Series.Clear();
        //    //CandlesChart.Series.Add(series);

        //    double i = 1;
        //    foreach (var oneCandle in candlesResponse.Candles.Take(50))
        //    {
        //        var time = DateTimeAxis.ToDouble(oneCandle.Date);
        //        series.Append(new OhlcvItem(time, oneCandle.Open, oneCandle.High, oneCandle.Low, oneCandle.Close));
        //        i++;
        //    }
        //    OnPropertyChanged("CandlesChart");
        //    //series.Append(new OhlcvItem(1, 3, 5, 1, 4));
        //    //series.Append(new OhlcvItem(2, 4, 5, 1, 3));
        //    //Random random = new Random();
        //    //for (int i = 3; i < 20; i++)
        //    //{
        //    //    var item = random.Next(1, i);
        //    //    series.Append(new OhlcvItem(i, item, item + 10, item - 3, item + 1));
        //    //}

        //    PlotModel plotModel = new PlotModel();
        //    //CandlesChart.Series.Add(series);
        //    plotModel.Series.Add(series);
        //    CandlesChart = plotModel;


        //}
        private async void FillPlotByNewData(CandlesResponse candlesResponse)
        {
            SeriesCollection = new SeriesCollection(); //если хочешь обновлять данные, то удали это
            SeriesVolumeCollection = new SeriesCollection();
            Labels = new List<string>();


            var series = new CandleSeries()
            {
                Values = new ChartValues<OhlcPoint>(),
            };

            var volumeSeries = new LiveCharts.Wpf.ColumnSeries()
            {
                Title = "Объем",
                Values = new ChartValues<double>()
            };

            //if (SeriesCollection.Any()) // можно спокойно добавлять объекты в коллекцию
            //series = (CandleSeries)SeriesCollection[0];
            var skipCnt = candlesResponse.Candles.Count - 100;
            candlesResponse.Candles = candlesResponse.Candles.Skip(skipCnt).Take(100).ToList();

            var labels = await FillSeriesBeforeShow(candlesResponse, series, volumeSeries);

            MaxAxesVal = candlesResponse.Candles.Count;
            //MaxAxesVal = 30;
            MinAxesVal = MaxAxesVal - 30;

            //if (!seriesCollection.Any())
            //    SeriesCollection.Add(series);
            //else
            //    SeriesCollection[0] = series;

            SeriesCollection.Add(series);
            SeriesVolumeCollection.Add(volumeSeries);
            Labels = labels;
            MaxAxesVal = candlesResponse.Candles.Count;
            MinAxesVal = MaxAxesVal - 30;
        }

        private Task<List<string>> FillSeriesBeforeShow(CandlesResponse candlesResponse, CandleSeries candleSeries, ColumnSeries volumeSeries)
        {
            var task = new Task<List<string>>(
                () =>
                {
                    //var series = new CandleSeries()
                    //{
                    //    Values = new ChartValues<OhlcPoint>(),
                    //};

                    //var volumeSeries = new LiveCharts.Wpf.ColumnSeries()
                    //{
                    //    Title = "Объем",
                    //    Values = new ChartValues<double>()
                    //};
                    var firstCandle = candlesResponse.Candles.First();
                    var labels = new List<string>();
                    var dtFormat = "dd.MM.yyyy HH:mm";
                    if (firstCandle.Interval == CandlesInterval.Day || firstCandle.Interval == CandlesInterval.Week || firstCandle.Interval == CandlesInterval.Month)
                        dtFormat = "dd.MM.yyyy";

                    //GearedValues
                    foreach (var oneCandle in candlesResponse.Candles)
                    {
                        candleSeries.Values.Add(new OhlcPoint(oneCandle.Open, oneCandle.High, oneCandle.Low, oneCandle.Close));
                        volumeSeries.Values.Add((double)oneCandle.Volume);
                        labels.Add(oneCandle.Date.ToString(dtFormat));
                    }

                    return labels;
                });

            task.Start();
            return task;
        }


        private string TranslatePredictionDecision(string oldValue)
        {
            switch (oldValue)
            {
                case "Sell": return "Продавать";
                case "Buy": return "Покупать";
                case "Neutral": return "Держать";
                default: return oldValue;
            }
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
