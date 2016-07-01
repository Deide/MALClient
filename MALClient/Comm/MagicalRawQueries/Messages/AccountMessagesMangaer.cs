using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models;

namespace MALClient.Comm.MagicalRawQueries.Messages
{
    public static class AccountMessagesManager
    {
        private static readonly Dictionary<int,List<MalMessageModel>> AllMessagesPaginated = new Dictionary<int, List<MalMessageModel>>();
        //private static readonly Dictionary<int,List<MalMessageModel>> AllSentMessagesPaginated = new Dictionary<int, List<MalMessageModel>>();
        private static int MaxPage { get; set; } = 9999;
        //private static int MaxSentPage { get; set; } = 9999;
        public static async Task<List<MalMessageModel>> GetMessagesAsync(int page)
        {
            if(page >= MaxPage)
                throw new ArgumentOutOfRangeException();

            if (AllMessagesPaginated.ContainsKey(page))
                return AllMessagesPaginated[page];

            AllMessagesPaginated[page] = await new MessagesQuery().GetMessages(page);

            if (AllMessagesPaginated[page].Count != 0)
                return AllMessagesPaginated[page];

            MaxPage = page;
            throw new ArgumentOutOfRangeException();
        }

        //public static async Task<List<MalMessageModel>> GetSentMessagesAsync(int page)
        //{
        //    if(page >= MaxSentPage)
        //        throw new ArgumentOutOfRangeException();

        //    if (AllSentMessagesPaginated.ContainsKey(page))
        //        return AllSentMessagesPaginated[page];

        //    AllSentMessagesPaginated[page] = await new MessagesQuery().GetSentMessages(page);

        //    if (AllSentMessagesPaginated[page].Count != 0)
        //        return AllSentMessagesPaginated[page];

        //    MaxSentPage = page;
        //    throw new ArgumentOutOfRangeException();
        //}
    }
}
