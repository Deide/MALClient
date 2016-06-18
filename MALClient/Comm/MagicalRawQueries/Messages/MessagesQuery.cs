using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm.MagicalRawQueries.Messages
{
    class MessagesQuery
    {
        public async Task<List<MalMessageModel>> GetMessages(int page = 1)
        {
            var client = await MalHttpContextProvider.GetHttpContextAsync();
            string path = $"/mymessages.php?go=&show={page*20 - 20}";
            var res = await client.GetAsync(path);
            var body = await res.Content.ReadAsStringAsync();
            

            var output = new List<MalMessageModel>();
            if (body.Contains("You have 0 messages"))
                return output;

            var doc = new HtmlDocument();
            doc.LoadHtml(body);
            foreach (var msgNode in doc.WhereOfDescendantsWithClass("div", "message read spot1 clearfix").Union(doc.WhereOfDescendantsWithClass("div", "message unread spot1 clearfix")))
            {
                var current = new MalMessageModel();
                current.Sender = msgNode.FirstOfDescendantsWithClass("div", "mym mym_user").InnerText.Trim();
                var contentNode = msgNode.FirstOfDescendantsWithClass("div", "mym mym_subject");
                current.Subject = WebUtility.HtmlDecode(contentNode.Descendants("a").First().ChildNodes[0].InnerText.Trim().Trim('-'));
                current.Content = WebUtility.HtmlDecode(contentNode.Descendants("span").First().InnerText.Trim());
                current.Id =
                    contentNode.FirstOfDescendantsWithClass("a", "subject-link").Attributes["href"].Value.Split('=')
                        .Last();
                current.Date = msgNode.FirstOfDescendantsWithClass("span", "mym_date").InnerText.Trim();
                output.Add(current);
            }

            return output;
        }
    }
}
