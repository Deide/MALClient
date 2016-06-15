using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm.MagicalRawQueries.Messages
{
    class MessagesQuery
    {
        public async Task<string> GetMessages()
        {
            string output = "";

            var baseAddress = new Uri("http://myanimelist.net");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler {CookieContainer = cookieContainer,UseCookies = true})
            using (var client = new HttpClient(handler) {BaseAddress = baseAddress})
            {
                var contentPairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_name", Credentials.UserName),
                    new KeyValuePair<string, string>("password", Credentials.Password),
                    new KeyValuePair<string, string>("submit", "Login"),
                    new KeyValuePair<string, string>("csrf_token", await CsrfTokenManager.GetToken())
                };
                var content = new FormUrlEncodedContent(contentPairs);

                var response = await client.PostAsync("/login.php", content);
                if (response.IsSuccessStatusCode)
                {
                    var res = await client.GetAsync("/mymessages.php");
                    var charCont = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("cid", "118763"),
                        new KeyValuePair<string, string>("csrf_token", await CsrfTokenManager.GetToken())
                    };
                    var contentchar = new FormUrlEncodedContent(charCont);
                    var reschar = await client.PostAsync("/includes/ajax.inc.php?s=1&t=42", contentchar);

                    return await res.Content.ReadAsStringAsync();
                }
            }



            return output;
        }
    }
}
