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
        public async Task<bool> SendMessage(string subject, string message)
        {
            var client = new HttpClient();
            var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("subject",subject),
                new KeyValuePair<string, string>("message",message),
                new KeyValuePair<string, string>("csrf_token",await CsrfTokenManager.GetToken()),
                new KeyValuePair<string, string>("sendmessage","Send Message")
            };
            var content = new FormUrlEncodedContent(contentPairs);

            var response = await client.PostAsync("http://myanimelist.net/mymessages.php?go=send&toname=zero_omar", content);

            return response.IsSuccessStatusCode;
        }
    }
}
