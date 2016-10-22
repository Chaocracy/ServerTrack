using ServerTrack.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerTrack
{
    public class ServerLoadController : ApiController
    {
        [Route("{serverName}/{cpu}/{ram}")]
        public bool RecordServerLoad(string serverName, double cpu, double ram)
        {
            return RecordManager.RecordLoad(DateTimeOffset.Now, serverName, cpu, ram);
        }
        [Route("{serverName}/minutes")]
        public IEnumerable<ServerLoadRecord> GetServerLoadByMinute(string serverName)
        {
            ServerRecord serverRecord;
            if (RecordManager.Servers.TryGetValue(serverName, out serverRecord))
            {
                return serverRecord.AverageLoadByMinute;
            }
            return null;
        }
        [Route("{serverName}/hours")]
        public IEnumerable<ServerLoadRecord> GetServerLoadByHour(string serverName)
        {
            ServerRecord serverRecord;
            if (RecordManager.Servers.TryGetValue(serverName, out serverRecord))
            {
                return serverRecord.AverageLoadByHour;
            }
            return null;
        }
    }
}
