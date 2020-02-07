using ExchCommonLib.Classes.Exchange;
using ExchCommonLib.Classes.Requests;
using ExchCommonLib.Classes.Responses;
using ExchCommonLib.Rest;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using SmaDesktopTerminal.Classes;
using SmaDesktopTerminal.Classes.ServiceResponses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ExchCommonLib.Analytics;
using ExchCommonLib.Classes;
using ExchCommonLib.Classes.Operations;
using ExchCommonLib.Classes.UserPortfolio;
using ExchCommonLib.Enums;
using SmaDesktopTerminal.Classes.Analytics;
using SmaDesktopTerminal.Classes.Interface;
using ColumnSeries = LiveCharts.Wpf.ColumnSeries;
using LiveCharts.Geared;
using LiveCharts.Definitions.Series;
using SmaDesktopTerminal.Classes.Indicators;
using TechAnalysisAlgLib.Indicators.MovingAverage;
using TechAnalysisAlgLib.Indicators.Oscillators;
using TechAnalysisAlgLib.Ml;

namespace SmaDesktopTerminal.Models
{
    public class TerminalWindowModel : INotifyPropertyChanged
    {
        private readonly AppMainModel mainModel;

        private readonly string urlToService;
        private readonly HttpClient httpClientService;
        private readonly DesktopTerminalWindow desktopTerminalWindow;

        public event PropertyChangedEventHandler PropertyChanged;

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
        private string selectedChartType;
        private List<string> chartTypes;
        private readonly System.Windows.Media.BrushConverter brushConverter;
        private CandlesResponse lastCandlesResponse;
        private ObservableCollection<ChartIndicatorInfo> chartIndicators;
        private Dictionary<string, string> availableChartIndicators;
        private KeyValuePair<string, string> selectedAvailableChartIndicator;
        private ICommand addIndicatorToChartCommand;
        private SeriesCollection seriesRsiCollection;
        private GridLength chartsListHeight;
        private ObservableCollection<ChartInfo> chartsAll;

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

        public List<string> ChartTypes
        {
            get => chartTypes;
            set
            {
                chartTypes = value;
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


        public SeriesCollection SeriesRsiCollection
        {
            get => seriesRsiCollection;
            set
            {
                seriesRsiCollection = value;
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

        public string SelectedChartType
        {
            get => selectedChartType;
            set
            {
                selectedChartType = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<ChartIndicatorInfo> ChartIndicators
        {
            get => chartIndicators;
            set
            {
                chartIndicators = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, string> AvailableChartIndicators
        {
            get => availableChartIndicators;
            set
            {
                availableChartIndicators = value;
                OnPropertyChanged();
            }
        }

        public KeyValuePair<string, string> SelectedAvailableChartIndicator
        {
            get => selectedAvailableChartIndicator;
            set
            {
                selectedAvailableChartIndicator = value;
                OnPropertyChanged();
            }
        }


        public ICommand AddIndicatorToChartCommand
        {
            get => addIndicatorToChartCommand;
            set
            {
                addIndicatorToChartCommand = value;
                OnPropertyChanged();
            }
        }


        public GridLength ChartsListHeight
        {
            get => chartsListHeight;
            set
            {
                chartsListHeight = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChartInfo> ChartsAll
        {
            get => chartsAll;
            set
            {
                chartsAll = value;
                OnPropertyChanged();
            }
        }


        public TerminalWindowModel(AppMainModel mainModel, Person person)
        {
            this.mainModel = mainModel;
            urlToService = "http://localhost:4000/";


            httpClientService = new HttpClient();
            httpClientService.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", person.AuthToken);
            CurPerson = person;
            Instruments = new ObservableCollection<Instrument>();

            SeriesCollection = new SeriesCollection();
            SeriesVolumeCollection = new SeriesCollection();
            Labels = new List<string>();
            ChartsAll = new ObservableCollection<ChartInfo>();
            SeriesRsiCollection = new SeriesCollection();
            ChartsListHeight = new GridLength();

            AnalyseProgressBarVisibility = Visibility.Hidden;
            InstrumentsProgressBarVisibility = Visibility.Hidden;
            ChartProgressBarVisibility = Visibility.Hidden;
            brushConverter = new System.Windows.Media.BrushConverter();

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

            ChartIndicators = new ObservableCollection<ChartIndicatorInfo>();
            AddIndicatorToChartCommand = new ActionCommand(AddIndicatorToChart);
            desktopTerminalWindow = new DesktopTerminalWindow(this);
            desktopTerminalWindow.Show();

            InitComboBoxItems();
        }

        private void InitComboBoxItems()
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

            ChartTypes = new List<string>(){
            "Область",
            "Свечи",
            "Бары"
            };

            AvailableChartIndicators = new Dictionary<string, string>()
            {
                { "Скользящее среднее","SMA"},
                { "Экспоненциальное скользящее среднее","EMA"},
                { "Взвешенное скользящее среднее","WMA"},
                { "Моментум","Momentum"},
                { "Индекс относительной силы","RSI"},
                { "Чудесный осциллятор","AO"},
                { "Уровень MACD","MACD"},
                { "Процентный диапазон Вильямса","WPR"},
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

                    var dateStart = DateTime.Now.AddDays(-5);

                    if (selectedChartInterval == "day")
                        dateStart = DateTime.Now.AddDays(-70);

                    if (selectedChartInterval == "week")
                        dateStart = DateTime.Now.AddDays(-450);

                    if (selectedChartInterval == "month")
                        dateStart = DateTime.Now.AddMonths(-50);

                    if (selectedChartInterval == "hour")
                        dateStart = DateTime.Now.AddDays(-10);


                    var candlesRequest = new CandlesRequest()
                    {
                        InstrumentId = (uint)SelectedInstrument.FinamEmitentIDInt,
                        Interval = selectedChartInterval,
                        DateStart = dateStart,
                        DateEnd = DateTime.Now.AddDays(1)
                    };
                    var res = await CallRest.PostAsync<CandlesRequest, CandlesResponse>(
                        $"{urlToService}api/v1/market/candles",
                        candlesRequest, httpClientService);

                    if (candlesRequest.InstrumentId != (uint)SelectedInstrument.FinamEmitentIDInt)
                        return;

                    lastCandlesResponse = res?.Response;
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
                    var mlConsolidate = new ConsolidateInstrumentsAnalysis(res.Response.Predictions);
                    mlConsolidate.SetMlSummary(mlMethods?.FirstOrDefault()?.PredictionDecision);
                    MlAnalysisInfo = new InstrumentsAnalysisInfo(mlMethods)
                    {
                        ConsolidateAnalysisInfo = mlConsolidate
                    };


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

        public void AddIndicatorToChart()
        {
            if (selectedAvailableChartIndicator.Value == "SMA")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("SMA", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new SimpleMovingAverage() };
                ChartIndicators.Add(chartIndicatorInfo);
            }

            if (selectedAvailableChartIndicator.Value == "EMA")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("EMA", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new ExponentialMovingAverage(9) };
                ChartIndicators.Add(chartIndicatorInfo);
            }

            if (selectedAvailableChartIndicator.Value == "WMA")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("WMA", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new WeightedMovingAverage(9) };
                ChartIndicators.Add(chartIndicatorInfo);
            }

            if (selectedAvailableChartIndicator.Value == "AO")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("AO", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new AwesomeOscillator() };
                ChartIndicators.Add(chartIndicatorInfo);
            }

            if (selectedAvailableChartIndicator.Value == "RSI")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("RSI", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new RelativeStrengthIndex(14) };
                ChartIndicators.Add(chartIndicatorInfo);
            }

            if (selectedAvailableChartIndicator.Value == "Momentum")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("Momentum", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new Momentum() };
                ChartIndicators.Add(chartIndicatorInfo);
            }

            if (selectedAvailableChartIndicator.Value == "MACD")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("MACD", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new Macd() };
                ChartIndicators.Add(chartIndicatorInfo);
            }

            if (selectedAvailableChartIndicator.Value == "WPR")
            {
                var chartIndicatorInfo = new ChartIndicatorInfo("WPR", DeleteIndicatorFromChart, ChartRefresh) { Indicator = new WilliamsPercentRange() };
                ChartIndicators.Add(chartIndicatorInfo);
            }


            ChartRefresh();
        }

        private async void FillPlotByNewData(CandlesResponse candlesResponse)
        {
            //SeriesCollection = new SeriesCollection(); //если хочешь обновлять данные, то удали это
            //var mainChartSeriesCollection = new SeriesCollection(); //если хочешь обновлять данные, то удали это
            //SeriesVolumeCollection = new SeriesCollection();
            //Labels = new List<string>();


            ChartsAll.Clear();

            //var actualHeight = desktopTerminalWindow.MainChartsRow.ActualHeight;
            var mainChart = new ChartInfo(this)
            {
                SeriesCollection = new SeriesCollection()
            };


            var volumeChart = new ChartInfo(this)
            {
                Height = 100d,
                SeriesCollection = new SeriesCollection()
            };


            foreach (var oneIndicator in ChartIndicators)
            {
                oneIndicator.InitSeriesCollection();
            }



            ISeriesView series = new GCandleSeries()
            {
                Values = new GearedValues<OhlcPoint>(),
                Fill = System.Windows.Media.Brushes.Silver,
                Title = "",
            };

            if (SelectedChartType == "Область")
            {
                var brush = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#225885");
                if (brush != null) brush.Opacity = 0.5d;
                var brush2 = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#4682b4");

                series = new GLineSeries()
                {
                    Values = new GearedValues<double>(),
                    Stroke = brush2,
                    Fill = brush,
                    Title = "Цена"
                };
            }

            if (SelectedChartType == "Бары")
            {
                series = new GOhlcSeries()
                {
                    Title = "",
                    Values = new GearedValues<OhlcPoint>(),
                    Fill = System.Windows.Media.Brushes.Silver,
                };
            }

            var volumeSeries = new GColumnSeries()
            {
                Title = "Объем",
                Values = new GearedValues<double>()
            };



            //if (SeriesCollection.Any()) // можно спокойно добавлять объекты в коллекцию
            //series = (CandleSeries)SeriesCollection[0];

            var labels = await FillSeriesBeforeShow(candlesResponse, series, volumeSeries, SelectedChartType, ChartIndicators);

            MaxAxesVal = candlesResponse.Candles.Count;
            MinAxesVal = MaxAxesVal - 30;

            //if (!seriesCollection.Any())
            //    SeriesCollection.Add(series);
            //else
            //    SeriesCollection[0] = series;


            var charts = new List<ChartInfo>();

            mainChart.SeriesCollection.Add(series);
            volumeChart.SeriesCollection.Add(volumeSeries);

            charts.Add(mainChart);
            charts.Add(volumeChart);

            //mainChart.Lables = labels;
            //volumeChart.Lables = labels;


            SeriesCollection.Add(series);
            Labels = labels;
            MaxAxesVal = candlesResponse.Candles.Count;
            MinAxesVal = MaxAxesVal - 30;



            foreach (var oneIndicator in ChartIndicators)
            {
                switch (oneIndicator.IndicatorShortName)
                {
                    case "SMA":
                    case "EMA":
                    case "WMA":
                        {
                            mainChart.SeriesCollection.AddRange(oneIndicator.SeriesCollectionForIndicator);
                            break;
                        }
                    case "Momentum":
                    case "AO":
                    case "RSI":
                    case "MACD":
                    case "WPR":
                        {
                            var indicatorChart = new ChartInfo(this)
                            {
                                Height = 100d,
                                SeriesCollection = oneIndicator.SeriesCollectionForIndicator
                            };
                            charts.Add(indicatorChart);
                            break;
                        }
                }
            }

            charts.ForEach(r =>
            {
                r.Lables = labels;
                r.MaxAxesVal = MaxAxesVal;
                r.MinAxesVal = MinAxesVal;
                ChartsAll.Add(r);
            });

            UpdateChartsHeight();
        }

        private Task<List<string>> FillSeriesBeforeShow(CandlesResponse candlesResponse, ISeriesView candleSeries, GColumnSeries volumeSeries, string chartType, ObservableCollection<ChartIndicatorInfo> chartIndicators)
        {
            var task = new Task<List<string>>(
                () =>
                {

                    foreach (var oneIndicator in chartIndicators)
                    {
                        oneIndicator.CalculateIndicatorValuesAndFillSeries(candlesResponse.Candles, 100);
                    }

                    var skipCnt = candlesResponse.Candles.Count - 100;
                    candlesResponse.Candles = candlesResponse.Candles.Skip(skipCnt).Take(100).ToList();



                    var firstCandle = candlesResponse.Candles.First();
                    var labels = new List<string>();
                    var dtFormat = "dd.MM.yyyy HH:mm";
                    if (firstCandle.Interval == CandlesInterval.Day || firstCandle.Interval == CandlesInterval.Week || firstCandle.Interval == CandlesInterval.Month)
                        dtFormat = "dd.MM.yyyy";

                    //GearedValues
                    foreach (var oneCandle in candlesResponse.Candles)
                    {
                        if (chartType == "Область")
                        { candleSeries.Values.Add(oneCandle.Close); }
                        else
                        { candleSeries.Values.Add(new OhlcPoint(oneCandle.Open, oneCandle.High, oneCandle.Low, oneCandle.Close)); }

                        volumeSeries.Values.Add((double)oneCandle.Volume);
                        labels.Add(oneCandle.Date.ToString(dtFormat));
                    }

                    return labels;
                });

            task.Start();
            return task;
        }
        private Task<List<string>> FillSeriesBeforeShow2(CandlesResponse candlesResponse, GLineSeries candleSeries, ColumnSeries volumeSeries)
        {
            var task = new Task<List<string>>(
                () =>
                {

                    var firstCandle = candlesResponse.Candles.First();
                    var labels = new List<string>();
                    var dtFormat = "dd.MM.yyyy HH:mm";
                    if (firstCandle.Interval == CandlesInterval.Day || firstCandle.Interval == CandlesInterval.Week || firstCandle.Interval == CandlesInterval.Month)
                        dtFormat = "dd.MM.yyyy";

                    foreach (var oneCandle in candlesResponse.Candles)
                    {
                        //candleSeries.Values.Add(new OhlcPoint(oneCandle.Open, oneCandle.High, oneCandle.Low, oneCandle.Close));
                        candleSeries.Values.Add(oneCandle.Close);
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


        public void DeleteIndicatorFromChart(ChartIndicatorInfo indicatorInfo)
        {

            try
            {
                ChartIndicators.Remove(indicatorInfo);
                var toRemove = ChartsAll.Where(r => r.SeriesCollection.All(r2 => indicatorInfo.SeriesCollectionForIndicator.Contains(r2)))
                     .ToList();

                if (toRemove.Any())
                {
                    foreach (var oneToRemove in toRemove)
                    {
                        ChartsAll.Remove(oneToRemove);
                    }
                }
                else
                {
                    ChartRefresh();

                }
            }
            catch (Exception e)
            {

            }
        }



        private void SelectedChartTypeChanged()
        {

        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ShutDownApp()
        {
            mainModel.ShutDownApp();
        }

        public void UpdateChartsHeight()
        {
            var actualHeight = desktopTerminalWindow.MainChartsRow.ActualHeight;

            var countCharts = ChartsAll?.Count ?? 0;

            if (countCharts < 2)
                return;

            var heightMainChart = actualHeight - (countCharts - 1) * 115;
            if (heightMainChart < actualHeight / 2)
                heightMainChart = actualHeight / 2;

            chartsAll.First().Height = heightMainChart;

        }
    }
}
