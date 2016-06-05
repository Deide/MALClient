using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    class MalArticlesIndexQuery : Query
    {
        public MalArticlesIndexQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/featured"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<MalNewsUnitModel>> GetArticlesIndex()
        {
            var output = new List<MalNewsUnitModel>();
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            foreach (var newsUnit in doc.WhereOfDescendantsWithClass("div", "news-unit clearfix"))
            {
                var current = new MalNewsUnitModel();
                var img = newsUnit.Descendants("a").First();
                current.Url = img.Attributes["href"].Value;
                current.ImgUrl = img.Descendants("img").First().Attributes["src"].Value;
                var contentDivs = newsUnit.Descendants("div").ToList();
                current.Title = WebUtility.HtmlDecode(contentDivs[0].Descendants("p").First().InnerText.Trim());
                current.Highlight = WebUtility.HtmlDecode(contentDivs[1].InnerText.Trim());
                var infos = contentDivs[2].Descendants("p").ToList();
                current.Author = infos[0].InnerText.Trim();
                current.Views = infos[1].InnerText.Trim();
                try
                {
                    current.Tags = string.Join(", ", contentDivs[3].Descendants("a").Select(node => node.InnerText.Trim()));
                }
                catch (Exception)
                {
                    //no tags
                }
                
                output.Add(current);
            }

            return output;
        }

    }
}
