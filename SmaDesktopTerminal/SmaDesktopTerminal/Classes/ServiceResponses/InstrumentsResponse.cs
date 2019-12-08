using ExchCommonLib.Classes.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaDesktopTerminal.Classes.ServiceResponses
{
    public class InstrumentsResponse
    {
        public List<Instrument> Instruments { get; set; }

        public InstrumentsResponse()
        {
            Instruments = new List<Instrument>();
        }
    }
}
