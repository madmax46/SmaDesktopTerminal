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
using System.Windows;
using System.Windows.Threading;
using ExchCommonLib.Classes;
using ExchCommonLib.Classes.Operations;
using ExchCommonLib.Classes.UserPortfolio;
using ExchCommonLib.Enums;
using SmaDesktopTerminal.Classes.Analytics;
using SmaDesktopTerminal.Classes.Interface;
using ColumnSeries = LiveCharts.Wpf.ColumnSeries;

namespace SmaDesktopTerminal.Models
{
    public class TerminalWindowModel : INotifyPropertyChanged
    {
        private readonly AppMainModel mainModel;

        private readonly string urlToService;
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
        private Visibility analyseProgressBarVisibility;
        private Visibility instrumentsProgressBarVisibility;
        private Visibility chartProgressBarVisibility;
        private string selectedIntervalForAnalysis;
        private PortfolioUserController portfolioUserController;
        private OperationUserController operationUserControllerInst;
        private OperationsHistoryUserController operationHistoryControllerInst;
        private Candle lastSelInstrumentCandle;

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
                if (OperationUserControllerInst != null)
                    OperationUserControllerInst.Instruments = value;
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

        internal void LogOut()
        {
            desktopTerminalWindow.Hide();
            CurPerson = null;
            Instruments.Clear();
            SelectedInstrument = null;
            ChartIntervals.Clear();
            SeriesCollection = null;
            SeriesVolumeCollection = null;
            MovingAveragesInfo = null;
            OscillatorsInfo = null;
            MlAnalysisInfo = null;
            PortfolioUserControllerInst = null;
            OperationUserControllerInst = null;
            OperationHistoryControllerInst = null;
            LastSelInstrumentCandle = null;

            ThemesController.ApplyLightTheme();
            mainModel.TerminalLogOut();
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


        public Visibility AnalyseProgressBarVisibility
        {
            get => analyseProgressBarVisibility;
            set
            {
                analyseProgressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility InstrumentsProgressBarVisibility
        {
            get => instrumentsProgressBarVisibility;
            set
            {
                instrumentsProgressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ChartProgressBarVisibility
        {
            get => chartProgressBarVisibility;
            set
            {
                chartProgressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        public string SelectedIntervalForAnalysis
        {
            get => selectedIntervalForAnalysis;
            set
            {
                var isNeedReload = selectedIntervalForAnalysis != value;
                selectedIntervalForAnalysis = value;
                OnPropertyChanged();

                if (isNeedReload)
                    ReloadInstrumentAnalysis();
            }
        }

        public PortfolioUserController PortfolioUserControllerInst
        {
            get => portfolioUserController;
            set
            {
                portfolioUserController = value;
                OnPropertyChanged();
            }
        }

        public OperationUserController OperationUserControllerInst
        {
            get => operationUserControllerInst;
            set
            {
                operationUserControllerInst = value;
                OnPropertyChanged();
            }
        }

        public OperationsHistoryUserController OperationHistoryControllerInst
        {
            get => operationHistoryControllerInst;
            set
            {
                operationHistoryControllerInst = value;
                OnPropertyChanged();
            }
        }

        public Candle LastSelInstrumentCandle
        {
            get => lastSelInstrumentCandle;
            set
            {
                lastSelInstrumentCandle = value;
                OnPropertyChanged();
            }
        }

        public TerminalWindowModel(AppMainModel mainModel, Person person)
        {
            this.mainModel = mainModel;
            //var authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWFkbWF4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJVc2VySWQiOiIxIiwiRmlyc3ROYW1lIjoi0JzQsNC60YHQuNC8IiwiU2Vjb25kTmFtZSI6ItCT0LvRg9GI0LDQutC-0LIiLCJFbWFpbCI6ImdsdXNoYWtvdm1heEBtYWlsLnJ1IiwiaXNzIjoiU21hQXV0aFNlcnZpY2UiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0LyJ9.tGKQu2877h_8axW8LtOmCpZ_DZRThEsXyNmj269BS9g";
            urlToService = "http://localhost:4000/";


            httpClientService = new HttpClient();
            httpClientService.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", person.AuthToken);
            CurPerson = person;
            Instruments = new ObservableCollection<Instrument>();
            CandlesChart = new PlotModel();

            SeriesCollection = new SeriesCollection();
            SeriesVolumeCollection = new SeriesCollection();
            Labels = new List<string>();

            AnalyseProgressBarVisibility = Visibility.Hidden;
            InstrumentsProgressBarVisibility = Visibility.Hidden;
            ChartProgressBarVisibility = Visibility.Hidden;

            OperationUserControllerInst = new OperationUserController(Instruments)
            {
                BuyOperationToClick = new BuyOperationToClick(AddBuyOperation),
                SellOperationToClick = new SellOperationToClick(AddSellOperation),
                Date = DateTime.Now
            };

            OperationHistoryControllerInst = new OperationsHistoryUserController()
            {
                PortfolioProgressBarVisibility = Visibility.Hidden
            };

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


        public void WindowLoaded()
        {
            ReloadPortfolioTab();
            ReloadParsedInstrumentsAsync();
        }


        public void ReloadPortfolioTab()
        {
            ReloadUserPortfolioAsync();
            ReloadUserOperationsHistoryAsync();
        }

        public async void ReloadUserPortfolioAsync()
        {
            try
            {
                PortfolioUserControllerInst = new PortfolioUserController
                {
                    PortfolioProgressBarVisibility = Visibility.Visible
                };

                var res = await CallRest.GetAsync<Portfolio>(
                    $"{urlToService}api/v1/portfolio/get", httpClientService);

                if (res?.Response == null)
                    return;

                if (res?.Response?.Positions?.Any() == true)
                {
                    res.Response.Positions.ForEach(r => PortfolioUserControllerInst.Positions.Add(r));
                }

                PortfolioUserControllerInst.TotalAmount = res.Response.TotalAmount;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (PortfolioUserControllerInst?.PortfolioProgressBarVisibility != null)
                    PortfolioUserControllerInst.PortfolioProgressBarVisibility = Visibility.Hidden;
            }
        }

        public async void ReloadUserOperationsHistoryAsync()
        {
            try
            {
                OperationHistoryControllerInst = new OperationsHistoryUserController()
                {
                    PortfolioProgressBarVisibility = Visibility.Visible,
                    UpdateHistoryCommand = new ActionCommand(ReloadUserOperationsHistoryAsync)
                };

                var res = await CallRest.GetAsync<OperationsHistory>(
                    $"{urlToService}api/v1/portfolio/operations/history", httpClientService);

                if (res?.Response == null)
                    return;

                if (res?.Response?.Operations?.Any() == true)
                {
                    res.Response.Operations.ForEach(r => OperationHistoryControllerInst.Operations.Add(r));
                }
            }
            catch (Exception e)
            {


            }
            finally
            {
                if (OperationHistoryControllerInst?.PortfolioProgressBarVisibility != null)
                    OperationHistoryControllerInst.PortfolioProgressBarVisibility = Visibility.Hidden;
            }
        }

        public async void ReloadParsedInstrumentsAsync()
        {
            try
            {
                InstrumentsProgressBarVisibility = Visibility.Visible;
                //desktopTerminalWindow.InstrumentsProgressBar.Visibility = System.Windows.Visibility.Visible;
                instruments.Clear();
                var res = await CallRest.GetAsync<InstrumentsResponse>(
                    $"{urlToService}api/v1/market/GetParsedInstruments", httpClientService);

                if (res?.Response?.Instruments?.Any() == true)
                {
                    res.Response.Instruments.ForEach(r => Instruments.Add(r));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                InstrumentsProgressBarVisibility = Visibility.Hidden;
            }
        }
        public void ChartRefresh()
        {
            ReloadCandlesForChart();
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

                    ChartProgressBarVisibility = Visibility.Visible;

                    var candlesRequest = new CandlesRequest()
                    {
                        InstrumentId = (uint)SelectedInstrument.FinamEmitentIDInt,
                        Interval = selectedChartInterval,
                        DateStart = DateTime.Now.AddYears(-1),
                        DateEnd = DateTime.Now.AddDays(1)
                    };
                    var res = await CallRest.PostAsync<CandlesRequest, CandlesResponse>(
                        $"{urlToService}api/v1/market/candles",
                        candlesRequest, httpClientService);

                    if (candlesRequest.InstrumentId != (uint)SelectedInstrument.FinamEmitentIDInt)
                        return;

                    if (res?.Response?.Candles?.Any() == true)
                    {
                        LastSelInstrumentCandle = res.Response.LastCandle;
                        FillPlotByNewData(res.Response);
                    }

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    ChartProgressBarVisibility = Visibility.Hidden;
                }
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

                    AnalyseProgressBarVisibility = Visibility.Visible;

                    //desktopTerminalWindow.chartProgressBar.Visibility = System.Windows.Visibility.Visible;
                    var predictionRequest = new PredictionRequest()
                    {
                        InstrumentId = (uint)SelectedInstrument.FinamEmitentIDInt,
                        Interval = SelectedIntervalForAnalysis,
                    };

                    var res = await CallRest.PostAsync<PredictionRequest, PredictionResponse>(
                        $"{urlToService}api/v1/market/analytics/GetPredictionFor",
                        predictionRequest, httpClientService);
                    res.Response.Predictions.ForEach(r =>
                        r.PredictionDecision = TranslatePredictionDecision(r.PredictionDecision));

                    if (predictionRequest.InstrumentId != (uint)SelectedInstrument.FinamEmitentIDInt)
                        return;

                    var grouped = res.Response.Predictions.GroupBy(r => r.IndicatorType);
                    var dict = grouped
                        .ToDictionary(r => r.Key, r => r.ToList());


                    dict.TryGetValue(1, out var movingAvg);
                    MovingAveragesInfo = new InstrumentsAnalysisInfo(movingAvg);

                    dict.TryGetValue(2, out var oscillator);
                    OscillatorsInfo = new InstrumentsAnalysisInfo(oscillator);

                    dict.TryGetValue(3, out var mlMethods);
                    MlAnalysisInfo = new InstrumentsAnalysisInfo(mlMethods);

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    AnalyseProgressBarVisibility = Visibility.Hidden;

                }

                ///api/v{version}/market/candles
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
        public void InstrumentIntervalChanged()
        {
            ReloadCandlesForChart();
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
                case "Unknown": return "--";
                default: return oldValue;
            }
        }


        private void AddBuyOperation()
        {
            SaveOperation(OperationType.Buy);
        }

        private void AddSellOperation()
        {
            SaveOperation(OperationType.Sell);

        }

        private async void SaveOperation(OperationType operationType)
        {

            try
            {


                var marketOperation = new MarketOperation();
                marketOperation.Count = (int)OperationUserControllerInst.Count;
                marketOperation.InstrumentId = (uint)OperationUserControllerInst.SelectedInstrument.FinamEmitentIDInt;
                marketOperation.Price = OperationUserControllerInst.OperationPrice;
                marketOperation.OrderType = operationType;
                marketOperation.Date = OperationUserControllerInst.Date;

                var res = await CallRest.PostAsync<MarketOperation, string>(
                    $"{urlToService}api/v1/portfolio/operations/save",
                    marketOperation, httpClientService);

            }
            catch (Exception e)
            {

            }
            finally
            {
                ReloadPortfolioTab();
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
