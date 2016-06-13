using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm.MagicalRawQueries
{
    class CsrfLoginQuery : Query
    {
        public async Task<bool> AttemptLogin(string token)
        {
            var client = new HttpClient();
            var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("user_name",Credentials.UserName),
                new KeyValuePair<string, string>("password",Credentials.Password),
                new KeyValuePair<string, string>("submit","Login"),
                new KeyValuePair<string, string>("csrf_token",token)
            };
            var content = new FormUrlEncodedContent(contentPairs);

            var response = await client.PostAsync("http://myanimelist.net/login.php", content);

            return response.IsSuccessStatusCode;
        }
    }
}
