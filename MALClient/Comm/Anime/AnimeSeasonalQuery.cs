﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Items;
using MALClient.ViewModels;

namespace MALClient.Comm
{
    internal class AnimeSeasonalQuery : Query
    {
        private readonly bool _overriden;
        private readonly AnimeSeason _season;
        private static readonly Dictionary<string,List<SeasonalAnimeData>> _prevQueries = new Dictionary<string, List<SeasonalAnimeData>>();

        public AnimeSeasonalQuery(AnimeSeason season)
        {
            _season = season;
            _overriden = _season.Url != "http://myanimelist.net/anime/season";
            Request = WebRequest.Create(Uri.EscapeUriString(_season.Url));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime(bool force = false)
        {
            if (!force && _prevQueries.ContainsKey(_season.Url))
                return _prevQueries[_season.Url];
            //In memory of 1 hour of my life spent over debugging single '?' character... minute of silence
            var output = force || DataCache.SeasonalUrls?.Count == 0 //either force or urls are empty after update
                ? new List<SeasonalAnimeData>()
                : (await DataCache.RetrieveSeasonalData(_overriden ? _season.Name : "") ?? new List<SeasonalAnimeData>());
            //current season without suffix
            if (output.Count != 0) return output;
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;


            //Get season data - we are getting this only from current season
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);
                var mainNode =
                    doc.DocumentNode.Descendants("div")
                        .First(
                            node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value ==
                                HtmlClassMgr.ClassDefs["#Seasonal:mainNode:class"]);
                if (!_overriden)
                {
                    var seasonInfoNodes = doc.DocumentNode.Descendants("div").First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            HtmlClassMgr.ClassDefs["#Seasonal:seasonInfo:class"]).Descendants("li").ToList();
                    var seasonData = new Dictionary<string, string>();
                    for (var j = 1; j <= 4; j++)
                    {
                        try
                        {
                            seasonData.Add(seasonInfoNodes[j].Descendants("a").First().InnerText.Trim(),
                                seasonInfoNodes[j].Descendants("a").First().Attributes["href"].Value);

                            if (seasonInfoNodes[j].Descendants("a").First().Attributes["class"].Value ==
                                HtmlClassMgr.ClassDefs["#Seasonal:seasonInfoCurrent:class"])
                                seasonData.Add("current", j.ToString());
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                    DataCache.SaveSeasonalUrls(seasonData);
                }

                //Get anime data
                var nodes = mainNode.ChildNodes.Where(node => node.Name == "div").Take(Settings.SeasonalToPull);

                var i = 0;
                foreach (var htmlNode in nodes)
                {
                    if (htmlNode.Attributes["class"]?.Value != HtmlClassMgr.ClassDefs["#Seasonal:entryNode:class"])
                        continue;

                    if (htmlNode.Attributes["class"]?.Value != HtmlClassMgr.ClassDefs["#Seasonal:entryNode:class"])
                        continue;

                    var imageNode =
                        htmlNode.FirstOfDescendantsWithClass("div", HtmlClassMgr.ClassDefs["#Seasonal:entryNode:image:class"]);
                    var link = imageNode.ChildNodes.First(node => node.Name == "a").Attributes["href"].Value;
                    var img = imageNode.Attributes["data-bg"].Value;
                    var scoreTxt =
                        htmlNode.Descendants("span")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    HtmlClassMgr.ClassDefs["#Seasonal:entryNode:score:class"])
                            .InnerText;
                    var infoNode =
                        htmlNode.Descendants("div")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    HtmlClassMgr.ClassDefs["#Seasonal:entryNode:info:class"]);
                    int day;
                    string airStartDate = null;
                    try
                    {
                        var date = infoNode.ChildNodes[1].InnerText.Trim().Substring(0, 13).Replace(",", "");
                        var dateObj = DateTime.Parse(date);
                        day = (int) dateObj.DayOfWeek;
                        airStartDate = dateObj.ToString("yyyy-MM-dd");
                        day++;
                    }
                    catch (Exception)
                    {
                        day = -1;
                    }

                    float score;
                    if (!float.TryParse(scoreTxt, out score))
                        score = 0;
                    output.Add(new SeasonalAnimeData
                    {
                        Title = WebUtility.HtmlDecode(imageNode.InnerText.Trim()),
                        //there are some \n that we need to get rid of
                        Id = int.Parse(link.Substring(7).Split('/')[2]), //extracted from anime link
                        ImgUrl = img, // from image style attr it's between ( )
                        Score = score, //0 for N/A
                        Episodes =
                            htmlNode.Descendants("div")
                                .First(
                                    node =>
                                        node.Attributes.Contains("class") &&
                                        node.Attributes["class"].Value ==
                                        HtmlClassMgr.ClassDefs["#Seasonal:entryNode:eps:class"])
                                .Descendants("a")
                                .First()
                                .InnerText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0],
                        Index = i,
                        Genres = htmlNode.Descendants("div").First(node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Seasonal:entryNode:genres:class"])
                            .InnerText
                            .Replace('\n', ';')
                            .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                            .Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
                            .ToList(),
                        AirDay = day,
                        AirStartDate = airStartDate
                    });
                    i++;
                }
            }
            catch (Exception)
            {
                //sumthing
            }


            DataCache.SaveSeasonalData(output, _overriden ? _season.Name : "");
            if (_prevQueries.ContainsKey(_season.Url))
                _prevQueries[_season.Url] = output;
            else
                _prevQueries.Add(_season.Url, output);
            //We are done.
            return output;
        }
    }
}