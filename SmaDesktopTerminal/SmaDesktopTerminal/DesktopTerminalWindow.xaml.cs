using LiveCharts.Wpf;
using SmaDesktopTerminal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmaDesktopTerminal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DesktopTerminalWindow : Window
    {
        private readonly TerminalWindowModel windowModel;
        public DesktopTerminalWindow(TerminalWindowModel windowModel)
        {
            this.windowModel = windowModel;
            DataContext = windowModel;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            windowModel.ReloadParsedInstrumentsAsync();
        }

        private void resizeThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            //Move the Thumb to the mouse position during the drag operation
            //double yadjust = firstColumn. + e.VerticalChange;
            double xadjust = userControl.Width + e.HorizontalChange;
            if ((xadjust >= 0))
            {
                //firstColumn.Width = new GridLength(xadjust, firstColumn.Width.GridUnitType);
                userControl.Width = xadjust;
                //Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) +
                //                        e.HorizontalChange);
                //Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) +
                //                        e.VerticalChange);
                //changes.Text = "Size: " +
                //                myCanvasStretch.Width.ToString() +
                //                 ", " +
                //                myCanvasStretch.Height.ToString();
            }

        }

        private void parsedInstrView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            windowModel.InstrumentSelectionChanged();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            windowModel.WindowLoaded();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void instrumentsUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            windowModel.ReloadParsedInstrumentsAsync();
        }

        private void chartUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            windowModel.ChartRefresh();
        }

        private void Axis_RangeChanged(LiveCharts.Events.RangeChangedEventArgs eventArgs)
        {
            //liveChartOhlc.AxisX[0].SetRange(30, 30);
            //eventArgs.Range
            //sync the graphs



            //double max = ((Axis)eventArgs.Axis).MaxValue;
            //double min = max - 30;

            //this.liveChartOhlc.AxisX[0].MinValue = min;
            //this.liveChartOhlc.AxisX[0].MaxValue = max;

            //this.liveChartVolume.AxisX[0].MinValue = min;
            //this.liveChartVolume.AxisX[0].MaxValue = max;

            //Repeat for as many graphs as you have
        }

        private void ChartIntervalComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            windowModel.InstrumentIntervalChanged();
        }

        private void ButtonLogOut_OnClick(object sender, RoutedEventArgs e)
        {
            windowModel.LogOut();
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            try
            {
                int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
                if (listView.ActualWidth == Double.NaN)
                    listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                double remainingSpace = listView.ActualWidth;
                for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                    if (i != autoFillColumnIndex)
                        remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
                (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
            }
            catch (Exception e)
            {

            }
        }

        private void SwitchCheckControl_OnClickToCheckBox(object sender, bool isChecked)
        {
            if (isChecked)
                ThemesController.ApplyDarkTheme();
            else
                ThemesController.ApplyLightTheme();
        }

        private void PortfolioUpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            windowModel.ReloadUserPortfolioAsync();
        }

        private void PortfolioInstrView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OperationsHistoryView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
