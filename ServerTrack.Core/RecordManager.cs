using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTrack.Core
{
    public class RecordManager
    {
        public static ConcurrentDictionary<string, ServerRecord> Servers { get; set; } = new ConcurrentDictionary<string, ServerRecord>();
        public static bool RecordLoad(DateTimeOffset timestamp, string serverName, double cpuLoad, double ramLoad)
        {
            ServerRecord serverRecord;
            if (!Servers.TryGetValue(serverName, out serverRecord))
            {
                serverRecord = new ServerRecord
                {
                    ServerName = serverName
                };
                if (!Servers.TryAdd(serverName, serverRecord))
                {
                    return false;
                }
            }

            var newMinuteRecord = new ServerLoadRecord
            {
                CpuLoad = cpuLoad,
                RamLoad = ramLoad,
                Timestamp = timestamp
            };
            serverRecord.ServerLoadRecords.Enqueue(newMinuteRecord);

            PurgeMinutes(timestamp, serverRecord);
            return true;
        }

        private static void PurgeMinutes(DateTimeOffset timestamp, ServerRecord serverRecord)
        {
            var oldestMinuteRecord = serverRecord.FirstMinuteRecord;
            if (serverRecord.FirstMinuteRecord != null && timestamp - serverRecord.FirstMinuteRecord > TimeSpan.FromDays(1))
            {
                ServerLoadRecord minuteClearingRecord;
                while (serverRecord.ServerLoadRecords.TryDequeue(out minuteClearingRecord))
                {
                    oldestMinuteRecord = serverRecord.FirstMinuteRecord;
                    if ((oldestMinuteRecord != null) && !(timestamp - oldestMinuteRecord > TimeSpan.FromDays(1)))
                    {
                        break;
                    }
                }
            }
        }
    }
}
