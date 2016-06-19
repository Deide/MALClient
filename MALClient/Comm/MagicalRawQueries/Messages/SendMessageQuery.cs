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
        /// <summary>
        /// Send new message.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SendMessage(string subject, string message, string targetUser)
        {
            var client = await MalHttpContextProvider.GetHttpContextAsync();

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

        /// <summary>
        /// Send reply message.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SendMessage(string subject, string message, string targetUser,string threadId,string replyId)
        {
            var client = await MalHttpContextProvider.GetHttpContextAsync();

            var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("csrf_token", client.Token),
                new KeyValuePair<string, string>("sendmessage", "Send Message")
            };
            var content = new FormUrlEncodedContent(contentPairs);

            var response =
                await client.PostAsync($"/mymessages.php?go=send&replyid={replyId}&threadid={threadId}&toname={targetUser}", content);

            return response.IsSuccessStatusCode;
        }
    }
}
