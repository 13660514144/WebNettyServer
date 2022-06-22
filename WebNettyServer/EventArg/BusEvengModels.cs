using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebNettyServer
{
    public class BusEvengModels
    {
        public int Code { get; set; } = 200;
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}
