using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ExchCommonLib.Analytics;
using ExchCommonLib.Classes;

namespace SmaDesktopTerminal.Classes.Analytics
{
    public class InstrumentsAnalysisInfo : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private List<AnalyticalPredictionInfo> predictions;
        private ConsolidateInstrumentsAnalysis consolidateAnalysisInfo;

        public List<AnalyticalPredictionInfo> Predictions
        {
            get { return predictions; }

            set
            {
                predictions = value;
                OnPropertyChanged();
            }
        }


        public ConsolidateInstrumentsAnalysis ConsolidateAnalysisInfo
        {
            get => consolidateAnalysisInfo;
            set
            {
                consolidateAnalysisInfo = value;
                OnPropertyChanged();
            }
        }

        public InstrumentsAnalysisInfo()
        {
            Predictions = new List<AnalyticalPredictionInfo>();
            ConsolidateAnalysisInfo = new ConsolidateInstrumentsAnalysis(Predictions);
        }


        public InstrumentsAnalysisInfo(List<AnalyticalPredictionInfo> predictions)
        {
            Predictions = predictions;
            ConsolidateAnalysisInfo = new ConsolidateInstrumentsAnalysis(predictions);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
