using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models;

namespace MALClient.Comm.MagicalRawQueries.Messages
{
    class MessagesQuery
    {
        public async Task<List<MalMessageModel>> GetMessages()
        {
            string body = "";
         
            using (var client = await MalHttpContextProvider.GetHttpContextAsync())
            {
                    var res = await client.GetAsync("/mymessages.php");
                    body = await res.Content.ReadAsStringAsync();
            }


        }
    }
}
