﻿using ExchCommonLib.Classes.Exchange;
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


        public async void ReloadCandlesAsync()
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
                    var res = await CallRest.PostAsync<CandlesRequest, CandlesResponse>($"{urlToService}api/v1/market/candles", candlesRequest, httpClientService);

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
        private void FillPlotByNewData(CandlesResponse candlesResponse)
        {
            //SeriesCollection = new SeriesCollection();
            //SeriesVolumeCollection = new SeriesCollection();
            //Labels = new List<string>();

            SeriesCollection = new SeriesCollection();
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

            var labels = new List<string>();

            //  VolumeStyle volumeStyle = VolumeStyle.Stacked;
            //var series = new CandleStickAndVolumeSeries
            //{
            //    PositiveColor = OxyColors.Green,
            //    NegativeColor = OxyColors.Red,
            //    PositiveHollow = false,
            //    NegativeHollow = false,
            //    SeparatorColor = OxyColors.Gray,
            //    SeparatorLineStyle = LineStyle.Dash,
            //    VolumeStyle = volumeStyle,
            //    //StrokeThickness = 3
            //};
            //CandlesChart.Series.Clear();
            //CandlesChart.Series.Add(series);
            //series.
            //GearedValues
            //desktopTerminalWindow.liveChartOhlc. = ScrollMode.X;


            //GearedValues

            foreach (var oneCandle in candlesResponse.Candles.Take(100))
            {
                var time = DateTimeAxis.ToDouble(oneCandle.Date);
                series.Values.Add(new OhlcPoint(oneCandle.Open, oneCandle.High, oneCandle.Low, oneCandle.Close));
                volumeSeries.Values.Add((double)oneCandle.Volume);
                labels.Add(oneCandle.Date.ToString());
            }

            SeriesCollection.Add(series);
            SeriesVolumeCollection.Add(volumeSeries);
            Labels = labels;
            //desktopTerminalWindow.liveChartOhlc.AxisX[0].MinValue = desktopTerminalWindow.liveChartOhlc.AxisX[0].MaxValue - 1;

            //desktopTerminalWindow.liveChartOhlc.AxisX[0].SetRange(30, 30);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
