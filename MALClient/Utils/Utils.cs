﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Media.Imaging;
using HtmlAgilityPack;
using MALClient.Comm;
using MALClient.Pages;
using MALClient.UserControls;
using MALClient.ViewModels;

namespace MALClient
{
    public static class Utils
    {
        private static readonly string[] SizeSuffixes = {"B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public static string StatusToString(int status, bool manga = false)
        {
            switch (status)
            {
                case 1:
                    return manga ? "Reading" : "Watching";
                case 2:
                    return "Completed";
                case 3:
                    return "On hold";
                case 4:
                    return "Dropped";
                case 6:
                    return manga ? "Plan to read" : "Plan to watch";
                case 7:
                    return "All";
                case 8:
                    return "Airing";
                default:
                    return "Not Set";
            }
        }

        public static string StatusToShortString(int status, bool manga = false)
        {
            switch (status)
            {
                case 1:
                    return manga ? "R" : "W";
                case 2:
                    return "C";
                case 3:
                    return "H";
                case 4:
                    return "D";
                case 6:
                    return "P";
                case 7:
                    return "";
                case 8:
                    return "";
                default:
                    return "N/A";
            }
        }

        public static int StatusToInt(string status)
        {
            switch (status)
            {
                case "Reading":
                case "Watching":
                    return 1;
                case "Completed":
                    return 2;
                case "On hold":
                    return 3;
                case "Dropped":
                    return 4;
                case "Plan to read":
                case "Plan to watch":
                    return 6;
                case "All":
                    return 7;
                case "Airing":
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string DayToString(DayOfWeek day,bool ignoreOffset = false)
        {
            if (day < 0)
                return "";
            if (Settings.AirDayOffset != 0 && !ignoreOffset)
            {
                var sum = Settings.AirDayOffset + (int) day;
                if (sum > 6)
                    day = (DayOfWeek) sum - 7;
                else if (sum < 0)
                    day = (DayOfWeek) 7 + sum;
                else
                    day += Settings.AirDayOffset;
            }
            switch (day)
            {
                case DayOfWeek.Friday:
                    return "Fri";
                case DayOfWeek.Monday:
                    return "Mon";
                case DayOfWeek.Saturday:
                    return "Sat";
                case DayOfWeek.Sunday:
                    return "Sun";
                case DayOfWeek.Thursday:
                    return "Thu";
                case DayOfWeek.Tuesday:
                    return "Tue";
                case DayOfWeek.Wednesday:
                    return "Wed";
                default:
                    return "";
            }
        }

        

        public static MainViewModel GetMainPageInstance()
        {
            return ViewModelLocator.Main;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static int ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = date.ToUniversalTime() - origin;
            return (int) Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        ///     http://stackoverflow.com/questions/28635208/retrieve-the-current-app-version-from-package
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersion()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        /// <summary>
        ///     http://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
        /// </summary>
        public static string SizeSuffix(long value)
        {
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }
            if (value == 0)
            {
                return "0.0 bytes";
            }

            var mag = (int) Math.Log(value, 1024);
            var adjustedSize = (decimal) value/(1L << (mag*10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public static async Task RemoveProfileImg()
        {
            try
            {
                await (await ApplicationData.Current.LocalFolder.GetFileAsync("UserImg.png")).DeleteAsync(
                    StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                //no file
            }
        }

        public static async Task DownloadProfileImg()
        {
            if (!Credentials.Authenticated)
                return;
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var thumb = await folder.CreateFileAsync("UserImg.png", CreationCollisionOption.ReplaceExisting);

                var http = new HttpClient();
                byte[] response = {};
                switch (Settings.SelectedApiType)
                {
                    case ApiType.Mal:
                        await
                            Task.Run(
                                async () =>
                                    response =
                                        await
                                            http.GetByteArrayAsync(
                                                $"http://cdn.myanimelist.net/images/userimages/{Credentials.Id}.jpg"));
                        break;
                    case ApiType.Hummingbird:
                        var avatarLink = await new ProfileQuery().GetHummingBirdAvatarUrl();
                        await Task.Run(async () => response = await http.GetByteArrayAsync(avatarLink));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //get bytes

                var fs = await thumb.OpenStreamForWriteAsync(); //get stream
                var writer = new DataWriter(fs.AsOutputStream());

                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();

                writer.Dispose();

                await ViewModelLocator.Hamburger.UpdateProfileImg(false);
            }
            catch (Exception)
            {
                //
            }
            await Task.Delay(2000);
            await ViewModelLocator.Hamburger.UpdateProfileImg(false);
        }

        public static async void DownloadCoverImage(string url, string title)
        {
            if (url == null)
                return;
            try
            {
                var sp = new FileSavePicker();
                sp.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                sp.FileTypeChoices.Add("Portable Network Graphics (*.png)", new List<string> {".png"});
                sp.SuggestedFileName = $"{title}-cover_art";

                var file = await sp.PickSaveFileAsync();
                if (file == null)
                    return;
                var http = new HttpClient();
                byte[] response = {};

                //get bytes
                await Task.Run(async () => response = await http.GetByteArrayAsync(url));


                var fs = await file.OpenStreamForWriteAsync(); //get stream
                var writer = new DataWriter(fs.AsOutputStream());

                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();

                writer.Dispose();
                GiveStatusBarFeedback("File saved successfully.");
            }
            catch (Exception)
            {
                GiveStatusBarFeedback("Error. File didn't save properly.");
            }
        }


        public static string CleanAnimeTitle(string title)
        {
            var index = title.IndexOf('+');
            return index == -1 ? title : title.Substring(0, index);
        }

        public static string FirstCharToUpper(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (var i = 0; i <= n; d[i, 0] = i++) ;
            for (var j = 1; j <= m; d[0, j] = j++) ;

            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    var min1 = d[i - 1, j] + 1;
                    var min2 = d[i, j - 1] + 1;
                    var min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        public static HamburgerButtons GetButtonForPage(PageIndex page)
        {
            switch (page)
            {
                case PageIndex.PageAnimeList:
                    return HamburgerButtons.AnimeList;
                case PageIndex.PageAnimeDetails:
                    return HamburgerButtons.AnimeList;
                case PageIndex.PageSettings:
                    return HamburgerButtons.Settings;
                case PageIndex.PageSearch:
                    return HamburgerButtons.AnimeSearch;
                case PageIndex.PageLogIn:
                    return HamburgerButtons.LogIn;
                case PageIndex.PageProfile:
                    return HamburgerButtons.Profile;
                case PageIndex.PageAbout:
                    return HamburgerButtons.About;
                case PageIndex.PageRecomendations:
                    return HamburgerButtons.Recommendations;
                case PageIndex.PageSeasonal:
                    return HamburgerButtons.Seasonal;
                case PageIndex.PageMangaList:
                    return HamburgerButtons.MangaList;
                case PageIndex.PageMangaSearch:
                    return HamburgerButtons.MangaSearch;
                case PageIndex.PageTopAnime:
                    return HamburgerButtons.TopAnime;
                case PageIndex.PageTopManga:
                    return HamburgerButtons.TopManga;
                case PageIndex.PageCalendar:
                    return HamburgerButtons.Calendar;
                case PageIndex.PageArticles:
                    return HamburgerButtons.Articles;
                case PageIndex.PageNews:
                    return HamburgerButtons.News;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
        }

        public static string DecodeXmlSynopsisDetail(string txt)
        {
            return
                Regex.Replace(txt, @"<[^>]+>|&nbsp;", "")
                    .Trim()
                    .Replace("[i]", "")
                    .Replace("[/i]", "")
                    .Replace("#039;", "'")
                    .Replace("&quot;", "\"")
                    .Replace("quot;", "\"")
                    .Replace("mdash;", "—")
                    .Replace("amp;", "&");
        }

        public static string DecodeXmlSynopsisSearch(string txt)
        {
            return
                Regex.Replace(txt, @"<[^>]+>|&nbsp;", "")
                    .Trim()
                    .Replace("[i]", "")
                    .Replace("[/i]", "")
                    .Replace("#039;", "'")
                    .Replace("&quot;", "\"")
                    .Replace("&mdash;", "—")
                    .Replace("&amp;", "&");
        }

        public static void GiveStatusBarFeedback(string text)
        {
        }

        public static string ShortDayToFullDay(string sub)
          {
              switch (sub)
              {
                  case "Fri":
                      return "Friday";
                  case "Mon":
                      return "Monday";
                  case "Sat":
                      return "Saturday";
                  case "Sun":
                      return "Sunday";
                  case "Thu":
                      return "Thursday";
                  case "Tue":
                      return "Tuesday";
                  case "Wed":
                      return "Wednesday";
                  default:
                      return "";
              }
          }

        public static HtmlNode FirstWithClass(this IEnumerable<HtmlNode> doc,string targettedClass)
        {
            return doc.First(
                node =>
                    node.Attributes.Contains("class") &&
                    node.Attributes["class"].Value ==
                    targettedClass);
        }

        public static HtmlNode FirstOfDescendantsWithClass(this HtmlDocument doc,string descendants, string targettedClass)
        {
            return doc.DocumentNode.Descendants(descendants)
                .First(
                    node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        targettedClass);
        }

        public static HtmlNode FirstOfDescendantsWithClass(this HtmlNode doc,string descendants, string targettedClass)
        {
            return doc.Descendants(descendants)
                .First(
                    node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        targettedClass);
        }

        public static IEnumerable<HtmlNode> WhereOfDescendantsWithClass(this HtmlDocument doc,string descendants,string targettedClass)
        {
            return doc.DocumentNode.Descendants(descendants)
                .Where(
                    node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        targettedClass);
        }

        public static IEnumerable<HtmlNode> WhereOfDescendantsWithClass(this HtmlNode doc, string descendants, string targettedClass)
        {
            return doc.Descendants(descendants).Where(
                node =>
                    node.Attributes.Contains("class") &&
                    node.Attributes["class"].Value ==
                    targettedClass);
        }

}
}