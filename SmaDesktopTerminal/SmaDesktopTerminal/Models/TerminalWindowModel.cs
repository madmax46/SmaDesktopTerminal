using ExchCommonLib.Classes.Exchange;
using ExchCommonLib.Rest;
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
using System.Threading.Tasks;

namespace SmaDesktopTerminal.Models
{
    public class TerminalWindowModel : INotifyPropertyChanged
    {
        private readonly AppMainModel mainModel;

        private readonly string urlToService;
        private readonly string authToken;
        private readonly HttpClient httpClientService;
        private Person person;
        private ObservableCollection<Instrument> instruments;

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

        public event PropertyChangedEventHandler PropertyChanged;


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


        public TerminalWindowModel(AppMainModel mainModel)
        {
            this.mainModel = mainModel;
            authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWFkbWF4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJpc3MiOiJTbWFBdXRoU2VydmljZSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3QvIn0.h5yBoP5P0GFuR-E2pv4fF-A1FQXcIy2HB_dvJ1vycA8";
            urlToService = "http://localhost:4000/";

            httpClientService = new HttpClient();
            httpClientService.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            CurPerson = mainModel.CurPerson;
            Instruments = new ObservableCollection<Instrument>();


            DesktopTerminalWindow desktopTerminalWindow = new DesktopTerminalWindow(this);
            desktopTerminalWindow.Show();

            ReloadParsedInstruments();
        }



        public void ReloadParsedInstruments()
        {
            try
            {
                var res = CallRest.GetAsync<InstrumentsResponse>($"{urlToService}api/v1/exchange/GetParsedInstruments", httpClientService).Result;

                if (res?.Response?.Instruments?.Any() == true)
                {
                    res.Response.Instruments.ForEach(r => Instruments.Add(r));
                }
            }
            catch (Exception ex)
            {

            }
        }





        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
