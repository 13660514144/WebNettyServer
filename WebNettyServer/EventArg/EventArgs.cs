using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebNettyServer
{
    public class BusMessageArgs : EventArgs
    {
        public BusMessageArgs(string comMessage)
        {
            this.ComMessage = comMessage;
        }
        public string ComMessage { get; set; }

    }
}
