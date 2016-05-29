﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;
using MALClient.Models.ApiResponses;
using MALClient.Models.Favourites;
using Newtonsoft.Json;

namespace MALClient.Comm
{
    public class ProfileQuery : Query
    {
        private string _userName;

        public ProfileQuery(bool feed = false,string userName = "")
        {
            if (string.IsNullOrEmpty(userName))
                userName = Credentials.UserName;
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request =
                        WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/profile/{userName}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    Request =
                        WebRequest.Create(
                            Uri.EscapeUriString(
                                $"https://hummingbird.me/api/v1/users/{Credentials.UserName}{(feed ? "/feed" : "")}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _userName = userName;
        }

        public async Task<ProfileData> GetProfileData(bool force = false)
        {
            var raw = await GetRequestResponse();
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var current = new ProfileData {User = {Name = _userName}};

            #region Recents
            try
            {


                var i = 1;
                foreach (
                    var recentNode in
                        doc.DocumentNode.Descendants("div")
                            .Where(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    HtmlClassMgr.ClassDefs["#Profile:recentUpdateNode:class"]))
                {
                    if (i <= 3)
                    {
                        current.RecentAnime.Add(
                            int.Parse(
                                recentNode.Descendants("a").First().Attributes["href"].Value.Substring(8).Split('/')[2]));
                    }
                    else
                    {
                        current.RecentManga.Add(
                            int.Parse(
                                recentNode.Descendants("a").First().Attributes["href"].Value.Substring(8).Split('/')[2]));
                    }
                    i++;
                }
            }
            catch (Exception)
            {
                //no recents
            }
            #endregion

            #region FavChar
            try
            {
                foreach (
                    var favCharNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    HtmlClassMgr.ClassDefs["#Profile:favCharacterNode:class"])
                            .Descendants("li"))
                {
                    var curr = new FavCharacter();
                    var imgNode = favCharNode.Descendants("a").First();
                    var styleString = imgNode.Attributes["style"].Value.Substring(22);
                    curr.ImgUrl = styleString.Replace("/r/80x120", "");
                    curr.ImgUrl = curr.ImgUrl.Substring(0, curr.ImgUrl.IndexOf('?'));
                    var infoNode = favCharNode.Descendants("div").Skip(1).First();
                    var nameNode = infoNode.Descendants("a").First();
                    curr.Name = nameNode.InnerText.Trim();
                    curr.Id = nameNode.Attributes["href"].Value.Substring(9).Split('/')[2];
                    var originNode = infoNode.Descendants("a").Skip(1).First();
                    curr.OriginatingShowName = originNode.InnerText.Trim();
                    curr.ShowId = originNode.Attributes["href"].Value.Split('/')[2];
                    curr.FromAnime = originNode.Attributes["href"].Value.Split('/')[1] == "anime";
                    current.FavouriteCharacters.Add(curr);
                }
            }
            catch (Exception)
            {
                //no favs
            }
            #endregion

            #region FavManga
            try
            {
                foreach (
                    var favMangaNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    HtmlClassMgr.ClassDefs["#Profile:favMangaNode:class"])
                            .Descendants("li"))
                {
                    current.FavouriteManga.Add(
                        int.Parse(
                            favMangaNode.Descendants("a").First().Attributes["href"].Value.Substring(9).Split('/')[2
                                ]));
                }
            }
            catch (Exception)
            {
                //no favs
            }
            #endregion

            #region FavAnime
            try
            {
                foreach (
                    var favAnimeNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    HtmlClassMgr.ClassDefs["#Profile:favAnimeNode:class"])
                            .Descendants("li"))
                {
                    current.FavouriteAnime.Add(
                        int.Parse(
                            favAnimeNode.Descendants("a").First().Attributes["href"].Value.Substring(9).Split('/')[2
                                ]));
                }
            }
            catch (Exception)
            {
                //no favs
            }

            #endregion

            #region FavPpl
            try
            {
                foreach (
                    var favPersonNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    HtmlClassMgr.ClassDefs["#Profile:favPeopleNode:class"])
                            .Descendants("li"))
                {
                    var curr = new FavPerson();
                    var aElems = favPersonNode.Descendants("a");
                    var styleString = aElems.First().Attributes["style"].Value.Substring(22);
                    curr.ImgUrl = styleString.Replace("/r/80x120", "");
                    curr.ImgUrl = curr.ImgUrl.Substring(0, curr.ImgUrl.IndexOf('?'));

                    curr.Name = aElems.Skip(1).First().InnerText.Trim();
                    curr.Id = aElems.Skip(1).First().Attributes["href"].Value.Substring(9).Split('/')[2];

                    current.FavouritePeople.Add(curr);
                }
            }
            catch (Exception)
            {
                //no favs
            }
            #endregion

            #region Stats
            try
            {
                var animeStats = doc.FirstOfDescendantsWithClass("div", "stats anime");
                var generalStats = animeStats.Descendants("div").First().Descendants("div");
                current.AnimeDays = float.Parse(generalStats.First().InnerText.Substring(5).Trim());
                current.AnimeMean = float.Parse(generalStats.Last().InnerText.Substring(11).Trim());
                int i = 0;
                #region AnimeStats
                foreach (
                        var htmlNode in
                            animeStats.FirstOfDescendantsWithClass("ul", "stats-status fl-l").Descendants("li"))
                {
                    switch (i)
                    {
                        case 0:
                            current.AnimeWatching =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;

                        case 1:
                            current.AnimeCompleted =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                        case 2:
                            current.AnimeOnHold =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                        case 3:
                            current.AnimeDropped =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                        case 4:
                            current.AnimePlanned =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                    }
                    i++;
                }
                //left stats done now right
                i = 0;
                foreach (var htmlNode in animeStats.FirstOfDescendantsWithClass("ul", "stats-data fl-r").Descendants("li"))
                {
                    switch (i)
                    {
                        case 0:
                            current.AnimeTotal =
                                     int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim().Replace(",", ""));
                            break;

                        case 1:
                            current.AnimeRewatched =
                                int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim().Replace(",", ""));
                            break;
                        case 2:
                            current.AnimeEpisodes =
                                int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim().Replace(",", ""));
                            break;
                    }
                    i++;
                }
                //we are done with anime
                #endregion
                i = 0;
                animeStats = doc.FirstOfDescendantsWithClass("div", "stats manga");
                generalStats = animeStats.Descendants("div").First().Descendants("div");
                current.MangaDays = float.Parse(generalStats.First().InnerText.Substring(5).Trim());
                current.MangaMean = float.Parse(generalStats.Last().InnerText.Substring(11).Trim());
                #region MangaStats
                foreach (
                        var htmlNode in
                            animeStats.FirstOfDescendantsWithClass("ul", "stats-status fl-l").Descendants("li"))
                {
                    switch (i)
                    {
                        case 0:
                            current.MangaReading =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;

                        case 1:
                            current.MangaCompleted =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                        case 2:
                            current.MangaOnHold =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                        case 3:
                            current.MangaDropped =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                        case 4:
                            current.MangaPlanned =
                                int.Parse(htmlNode.Descendants("span").First().InnerText.Trim().Replace(",", ""));
                            break;
                    }
                    i++;
                }
                //left stats done now right
                i = 0;
                foreach (var htmlNode in animeStats.FirstOfDescendantsWithClass("ul", "stats-data fl-r").Descendants("li"))
                {
                    switch (i)
                    {
                        case 0:
                            current.MangaTotal =
                                int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim().Replace(",", ""));
                            break;

                        case 1:
                            current.MangaReread =
                                int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim().Replace(",", ""));
                            break;
                        case 2:
                            current.MangaChapters =
                                int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim().Replace(",", ""));
                            break;
                        case 3:
                            current.MangaVolumes =
                                int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim().Replace(",", ""));
                            break;
                    }
                    i++;
                }
                //we are done with manga
                #endregion
            }
            catch (Exception)
            {
                //hatml
            }

            #endregion

            #region LeftSideBar

            var sideInfo = doc.FirstOfDescendantsWithClass("ul", "user-status border-top pb8 mb4").Descendants("li").ToList();

            current.LastOnline = sideInfo[0].LastChild.InnerText;
            current.Gender = sideInfo[1].LastChild.InnerText;
            current.Birthday = sideInfo[2].LastChild.InnerText;
            current.Location = sideInfo[3].LastChild.InnerText;
            current.Joined = sideInfo[4].LastChild.InnerText;
            current.User.ImgUrl =
                doc.FirstOfDescendantsWithClass("div", "user-image mb8").Descendants("img").First().Attributes["src"]
                    .Value;


            #endregion

            #region Friends

            var friends = doc.FirstOfDescendantsWithClass("div", "user-friends pt4 pb12").Descendants("a");
            foreach (var friend in friends)
            {
                var curr = new MalUser();
                var styleString = friend.Attributes["style"].Value.Substring(22);
                curr.ImgUrl = styleString.Replace("/r/76x120", "");
                curr.ImgUrl = curr.ImgUrl.Substring(0, curr.ImgUrl.IndexOf('?'));

                curr.Name = friend.InnerText;                
                current.Friends.Add(curr);
            }

            #endregion

            #region Comments

            var commentBox = doc.FirstOfDescendantsWithClass("div", "user-comments mt24 pt24");
            foreach (var comment in commentBox.WhereOfDescendantsWithClass("div", "comment clearfix"))
            {
                var curr = new MalComment();
                curr.User.ImgUrl = comment.Descendants("img").First().Attributes["src"].Value;
                var textBlock = comment.Descendants("div").First();
                var header = textBlock.Descendants("div").First();
                curr.User.Name = header.ChildNodes[1].InnerText;
                curr.Date = header.ChildNodes[3].InnerText;
                curr.Content = textBlock.Descendants("div").Skip(1).First().InnerText;
                current.Comments.Add(curr);
            }

            #endregion


            return current;


        }

        public async Task<string> GetHummingBirdAvatarUrl()
        {
            var raw = await GetRequestResponse();
            return ((dynamic) JsonConvert.DeserializeObject(raw)).avatar.ToString();
        }

        public async Task<HumProfileData> GetHumProfileData(bool force = false)
        {
            var raw = await GetRequestResponse();
            return JsonConvert.DeserializeObject<HumProfileData>(raw,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
        }

        public async Task<List<HumStoryObject>> GetHumFeedData(bool force = false)
        {
            var raw = await GetRequestResponse();
            return JsonConvert.DeserializeObject<List<HumStoryObject>>(raw,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
        }
    }
}