using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm.MagicalRawQueries.Messages
{
    public class SendMessageQuery
    {
        public async Task<bool> SendMessage(string subject, string message, string targetUser)
        {
            using (var client = await MalHttpContextProvider.GetHttpContextAsync())
            {
                var contentPairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("message", message),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                    new KeyValuePair<string, string>("sendmessage", "Send Message")
                };
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await client.PostAsync($"http://myanimelist.net/mymessages.php?go=send&toname={targetUser}", content);

                return response.IsSuccessStatusCode;
            }           
        }
    }
}
