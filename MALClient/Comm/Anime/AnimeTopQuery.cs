﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    public enum TopAnimeType
    {
        General,
        Airing,
        Upcoming,
        Tv,
        Movies,
        Ovas,
        Popular,
        Favourited,
        Manga
    }


    internal class AnimeTopQuery : Query
    {
        private static Dictionary<TopAnimeType, List<TopAnimeData>> _prevQueriesCache = new Dictionary<TopAnimeType, List<TopAnimeData>>();
        private TopAnimeType _type;
        private int _page;
        public AnimeTopQuery(TopAnimeType topType, int page = 0)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/{GetEndpoint(topType, page)}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _page = page;
            _type = topType;
        }

        private string GetEndpoint(TopAnimeType type, int page)
        {
            switch (type)
            {
                case TopAnimeType.General:
                    return $"topanime.php?limit={page * 50}";
                case TopAnimeType.Airing:
                    return $"topanime.php?type=airing&limit={page * 50}";
                case TopAnimeType.Upcoming:
                    return $"topanime.php?type=upcoming&limit={page * 50}";
                case TopAnimeType.Tv:
                    return $"topanime.php?type=tv&limit={page * 50}";
                case TopAnimeType.Movies:
                    return $"topanime.php?type=movie&limit={page * 50}";
                case TopAnimeType.Ovas:
                    return $"topanime.php?type=ova&limit={page * 50}";
                case TopAnimeType.Popular:
                    return $"topanime.php?type=bypopularity&limit={page * 50}";
                case TopAnimeType.Favourited:
                    return $"topanime.php?type=favorite&limit={page * 50}";
                case TopAnimeType.Manga:
                    return $"topmanga.php?limit={page * 50}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public async Task<List<TopAnimeData>> GetTopAnimeData(bool force = false)
        {
            if (!force)
                if (_prevQueriesCache.ContainsKey(_type))
                    return _prevQueriesCache[_type];

            var output = force ? new List<TopAnimeData>() : (await DataCache.RetrieveTopAnimeData(_type) ?? new List<TopAnimeData>());
            if (output.Count > 0)
            {
                _prevQueriesCache[_type] = output;
                return output;
            }
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return new List<TopAnimeData>();


            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var topNodes = doc.DocumentNode.Descendants("table").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Top:mainNode:class"]);
            var i = 50 * _page;
            string imgUrlType = _type == TopAnimeType.Manga ? "manga/" : "anime/";
            foreach (var item in topNodes.Descendants("tr").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Top:topNode:class"]))
            {
                try
                {
                    var current = new TopAnimeData();
                    var epsText = item.Descendants("div").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Top:topNode:eps:class"]).ChildNodes[0].InnerText;
                    epsText = epsText.Substring(epsText.IndexOf('(') + 1);
                    epsText = epsText.Substring(0, epsText.IndexOf(' '));
                    current.Episodes = epsText;
                    var img = item.Descendants("img").First().Attributes["data-src"].Value.Split('/');
                    int imgCount = img.Length;
                    var imgurl = img[imgCount - 2] + "/" + img[imgCount - 1];
                    var pos = imgurl.IndexOf('?');
                    if (pos != -1)
                        imgurl = imgurl.Substring(0, pos);
                    current.ImgUrl = "http://cdn.myanimelist.net/images/" + imgUrlType + imgurl;
                    var titleNode = item.Descendants("a").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs[_type != TopAnimeType.Manga ? "#Top:topNode:titleNode:class" : "#Top:topMangaNode:titleNode:class"]);
                    current.Title = WebUtility.HtmlDecode(titleNode.InnerText).Trim();
                    current.Id = Convert.ToInt32(titleNode.Attributes["href"].Value.Substring(7).Split('/')[2]);
                    try
                    {
                        current.Score = float.Parse(item.Descendants("span").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Top:topNode:score:class"]).InnerText.Trim());
                    }
                    catch (Exception)
                    {
                        current.Score = 0; //sometimes score in unavailable -> upcoming for example
                    }

                    current.Index = ++i;


                    output.Add(current);
                }
                catch (Exception)
                {
                    //
                }
            }
            if (_page != 0) //merge data
                output = _prevQueriesCache[_type].Union(output).Distinct().ToList();

            DataCache.SaveTopAnimeData(output, _type);
            _prevQueriesCache[_type] = output;
            return output;
        }
    }
}