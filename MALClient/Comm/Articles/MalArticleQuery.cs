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
        private string _title;
        public MalArticleQuery(string url,string title)
        {
            _title = title;
            Request =
                WebRequest.Create(Uri.EscapeUriString(url));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<string> GetArticleHtml()
        {
            var possibleData = await DataCache.RetrieveArticleContentData(_title);
            if (possibleData != null)
                return possibleData;
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var htmlData = doc.FirstOfDescendantsWithClass("div", "news-container").OuterHtml;
            DataCache.SaveArticleContentData(_title,htmlData);
            return htmlData;
        }
    }
}
