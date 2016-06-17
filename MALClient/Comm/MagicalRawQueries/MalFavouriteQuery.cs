using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm.MagicalRawQueries
{
    public enum FavouriteTypes
    {
        Anime,
        Manga,
        Character,
        Person
    }

    public class MalFavouriteQuery
    {
        public async void ModifyFavourite(int id,FavouriteTypes type,bool add)
        {
            string idFieldName;
            string actionId;
            switch (type)
            {
                case FavouriteTypes.Anime:
                    idFieldName = "aid";
                    actionId = add ? "13" : "14";
                    break;
                case FavouriteTypes.Manga:
                    idFieldName = "mid";
                    actionId = add ? "38" : "39";
                    break;
                case FavouriteTypes.Character:
                    idFieldName = "cid";
                    actionId = add ? "42" : "43";
                    break;
                case FavouriteTypes.Person:
                    idFieldName = "vaid";
                    actionId = add ? "47" : "48";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var client = await MalHttpContextProvider.GetHttpContextAsync();
            
            var charCont = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(idFieldName, id.ToString()),
                new KeyValuePair<string, string>("csrf_token", client.Token)
            };
            var contentchar = new FormUrlEncodedContent(charCont);
            await client.PostAsync($"/includes/ajax.inc.php?s=1&t={actionId}", contentchar);                 

        }
    }
}
