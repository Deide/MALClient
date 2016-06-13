using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm.MagicalRawQueries
{
    public static class CsrfTokenManager
    {
        private static string _token;
        private static DateTime? _tokenExpirationDate;
        /// <summary>
        /// Gets VALID token for user session, returns null if authentication failed or something.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetToken()
        {
            if (_tokenExpirationDate == null || DateTime.Now.CompareTo(_tokenExpirationDate.Value) > 0)//either we didn't have token yet or it has expired.        
            {
                var temp = await new CsrfTokenQuery().GetToken();
                bool authenticated = await new CsrfLoginQuery().AttemptLogin(temp);
                if (!authenticated)
                    return null;
                _token = temp;
                _tokenExpirationDate = DateTime.Now.Add(TimeSpan.FromHours(1)); // valid for one hour
            }
            return _token;
        }

    }
}
