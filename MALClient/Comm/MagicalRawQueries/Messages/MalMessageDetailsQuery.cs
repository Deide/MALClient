using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm.MagicalRawQueries.Messages
{
    public class MalMessageDetailsQuery
    {
        public async Task<MalMessageModel> GetMessageDetails(MalMessageModel msg)
        {
            var client = await MalHttpContextProvider.GetHttpContextAsync();
            var response = await client.GetAsync($"/mymessages.php?go=read&id={msg.Id}");
            var raw = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            var msgNode = doc.FirstOfDescendantsWithClass("td", "dialog-text");
            foreach (var descendant in msgNode.Descendants("div"))
                msgNode.RemoveChild(descendant);

            msg.Content = WebUtility.HtmlDecode(msgNode.InnerText.Trim());

            msg.ThreadId = doc.FirstOfDescendantsWithClass("div", "ac mt8 mb8").Descendants("a").First().Attributes["href"].Value.Split
                ('=').Last();

            return msg;
        }

        public async Task<List<MalMessageModel>> GetMessagesInThread(MalMessageModel msg)
        {
            var client = await MalHttpContextProvider.GetHttpContextAsync();
            var response = await client.GetAsync($"/mymessages.php?go=read&id={msg.Id}&threadid={msg.ThreadId}");
            var raw = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            var output = new List<MalMessageModel>();

            foreach (var msgHistoryNode in doc.FirstOfDescendantsWithClass("table", "pmessage-message-history").Descendants("tr"))
            {
                var current = new MalMessageModel();
                var tds = msgHistoryNode.Descendants("td").ToList();
                current.Date = tds[0].InnerText.Trim();
                current.Sender = tds[1].InnerText.Trim();
                current.Content = WebUtility.HtmlDecode(tds[2].InnerText.Trim());
                current.Subject = msg.Subject;
                output.Add(current);
            }


            return output;
        }
        
    }
}
