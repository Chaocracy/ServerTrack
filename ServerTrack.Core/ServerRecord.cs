using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTrack.Core
{
    public class ServerRecord
    {
        public string ServerName { get; set; }
        public ConcurrentQueue<ServerLoadRecord> ServerLoadRecords { get; set; } = new ConcurrentQueue<ServerLoadRecord>();
        public DateTimeOffset? FirstMinuteRecord
        {
            get
            {
                ServerLoadRecord firstMinute;
                if (ServerLoadRecords.TryPeek(out firstMinute))
                {
                    return firstMinute.Timestamp;
                }
                return null;
            }
        }
        public IEnumerable<ServerLoadRecord> AverageLoadByMinute
        {
            get
            {
                var groups = ServerLoadRecords.GroupBy(slr =>
                {
                    var realTimestamp = slr.Timestamp;
                    var newTimestamp = new DateTimeOffset(realTimestamp.Year, realTimestamp.Month, realTimestamp.Day, realTimestamp.Hour, realTimestamp.Minute, 0, realTimestamp.Offset);
                    return newTimestamp;
                })
                .Select(g => new ServerLoadRecord
                {
                    Timestamp = g.Key,
                    CpuLoad = g.Average(s => s.CpuLoad),
                    RamLoad = g.Average(s => s.RamLoad)
                })
                .Where(r => r.Timestamp > DateTimeOffset.Now.AddHours(-1));
                return groups;
            }
        }
        public IEnumerable<ServerLoadRecord> AverageLoadByHour
        {
            get
            {
                var groups = ServerLoadRecords.GroupBy(slr =>
                {
                    var realTimestamp = slr.Timestamp;
                    var newTimestamp = new DateTimeOffset(realTimestamp.Year, realTimestamp.Month, realTimestamp.Day, realTimestamp.Hour, 0, 0, realTimestamp.Offset);
                    return newTimestamp;
                })
                .Select(g => new ServerLoadRecord
                {
                    Timestamp = g.Key,
                    CpuLoad = g.Average(s => s.CpuLoad),
                    RamLoad = g.Average(s => s.RamLoad)
                })
                .Where(r => r.Timestamp > DateTimeOffset.Now.AddDays(-1));
                return groups;
            }
        }
    }
}
