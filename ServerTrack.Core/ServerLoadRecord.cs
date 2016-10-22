using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTrack.Core
{
    public class ServerLoadRecord
    {
        public double CpuLoad { get; set; }
        public double RamLoad { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
