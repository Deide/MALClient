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
            output.AddRange(doc.WhereOfDescendantsWithClass("div", "message unread spot1 clearfix").Select(msgNode => ParseHtmlToMalMessage(msgNode, false)));
            output.AddRange(doc.WhereOfDescendantsWithClass("div", "message read spot1 clearfix").Select(msgNode => ParseHtmlToMalMessage(msgNode, true)));


            return output;
        }

        private MalMessageModel ParseHtmlToMalMessage(HtmlNode msgNode,bool read)
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
            current.IsRead = read;

            var ids = msgNode.FirstOfDescendantsWithClass("span", "mym_actions").Descendants("a").First().Attributes["href"].Value.Split('=');
            current.ThreadId = ids[3].Substring(0, ids[3].IndexOf('&'));
            current.ReplyId = ids[2].Substring(0, ids[3].IndexOf('&'));
            return current;
        }
    }
}
