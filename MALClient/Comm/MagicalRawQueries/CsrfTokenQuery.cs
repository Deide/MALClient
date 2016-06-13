using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MALClient.Comm.MagicalRawQueries
{
    public class CsrfTokenQuery : Query
    {
        public CsrfTokenQuery()
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString("http://myanimelist.net/login.php"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<string> GetToken()
        {
            var raw = await GetRequestResponse();
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            var nodes = doc.DocumentNode.Descendants("head").First().ChildNodes;
            var csfr =
                nodes.First(
                    htmlNode =>
                        htmlNode.Attributes.Contains("name") && htmlNode.Attributes["name"].Value == "csrf_token")
                    .Attributes["content"].Value;

            return csfr;
        } 
    }
}
