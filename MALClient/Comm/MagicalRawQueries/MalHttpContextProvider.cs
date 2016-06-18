using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm.MagicalRawQueries
{
    /// <summary>
    /// Client wrapped with token.
    /// </summary>
    public class CsrfHttpClient : HttpClient
    {
        public string Token { get; set; }

        public CsrfHttpClient(HttpClientHandler hander) : base(hander)
        {
            
        }

        protected new void Dispose(bool disposing)
        {
            //it's not disposable
        }

        public void ExpiredDispose()
        {
            base.Dispose();
        }
    }

    public static class MalHttpContextProvider
    {
        private static string _token;
        private static CsrfHttpClient _httpClient;
        private static DateTime? _contextExpirationTime;
        private const string MalBaseUrl = "http://myanimelist.net";

        /// <summary>
        /// Establishes connection with MAL, attempts to authenticate.
        /// </summary>
        /// <param name="updateToken">
        /// Indicates whether created http client is meant to be used further or do we want to dispose it and return null.
        /// </param>
        /// <exception cref="WebException">
        /// Unable to authorize.
        /// </exception>
        /// <returns>
        /// Returns valid http client which can interact with website API.
        /// </returns>
        public static async Task<CsrfHttpClient> GetHttpContextAsync(bool updateToken = false)
        {
            if (_contextExpirationTime == null || DateTime.Now.CompareTo(_contextExpirationTime.Value) > 0)
            {
                _httpClient?.ExpiredDispose();

                var httpHandler = new HttpClientHandler {CookieContainer = new CookieContainer(), UseCookies = true,AllowAutoRedirect = false};
                var tempToken = await new CsrfTokenQuery().GetToken();
                var loginPostInfo = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_name", Credentials.UserName),
                    new KeyValuePair<string, string>("password", Credentials.Password),
                    new KeyValuePair<string, string>("submit", "Login"),
                    new KeyValuePair<string, string>("csrf_token", tempToken)
                };
                var content = new FormUrlEncodedContent(loginPostInfo);

                //we won't dispose it here because this instance gonna be passed further down to other queries
                _httpClient = new CsrfHttpClient(httpHandler) {BaseAddress = new Uri(MalBaseUrl)};               
                var response = await _httpClient.PostAsync("/login.php", content);
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found)
                {
                    _token = tempToken;
                    _httpClient.Token = tempToken;
                    _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                    if (updateToken) //we are here just to update this thing
                        return null;
                    return _httpClient; //else we are returning client that can be used for next queries
                }

                throw new WebException("Unable to authorize");
            }
            return _httpClient;
        }

        public static async Task<string> GetCsrfToken()
        {
            if (_contextExpirationTime == null || DateTime.Now.CompareTo(_contextExpirationTime.Value) > 0)
                await GetHttpContextAsync(true);
            return _token;
        }
    }
}
