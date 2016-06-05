using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MALClient.Comm.Articles
{
    public class MalArticleQuery : Query
    {
        public MalArticleQuery(string url)
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString(url));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<string> GetArticleHtml()
        {
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            return doc.FirstOfDescendantsWithClass("div", "news-container").OuterHtml;
        }
    }
}
