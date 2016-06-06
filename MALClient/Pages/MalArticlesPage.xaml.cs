﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps.LocalSearch;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MalArticlesPage : Page
    {
        public MalArticlesViewModel ViewModel => DataContext as MalArticlesViewModel;

        const string Begin = @"<html><body id='root'><div id='content'>";
        #region Css
        const string Css =
            @"<style type=""text/css"">@charset ""UTF-8"";
            ::-webkit-scrollbar { 
                display: none; 
            }
	        html, body
	        {
		        background-color: #2a2c2a;
		        color: white;
	        }
	        .userimg
	        {
		        display: block;
		        margin: 0 auto;
		        max-width: 100%;
		        height: auto;
		        -webkit-box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
		        -moz-box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
		        box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
	        }
	        a
	        {
		        font-weight: bold;
		        text-decoration: none;
	        }
            a:link{color:AccentColourBase}
            a:active{color:AccentColourBase}
            a:visited{color:AccentColourDark}
            a:hover{color:AccentColourLight}
        #root{
            height: 100%;
            width: 100%;
            overflow: hidden;
            position: relative;
        }

        #content{
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: -17px; /* Increase/Decrease this value for cross-browser compatibility */
            overflow-y: scroll;
        }
        h1 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 24px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 26.4px;
        }
        h2 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 24px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 26.4px;
        }
        h3 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 18px;
	        font-style: normal;
	        font-variant: normal;
	        position: relative;
	        text-align: center;
	        font-weight: 500;
	        line-height: 15.4px;
        }
        h4 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 14px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 15.4px;
        }
        hr {
            display: block;
            height: 2px;
	        background-color: #0d0d0d;
	        border-radius: 10px 10px 10px 10px;
	        -moz-border-radius: 10px 10px 10px 10px;
	        -webkit-border-radius: 10px 10px 10px 10px;
	        border: 1px solid #1f1f1f;
            margin: 1em 0;
            padding: 0; 
        }
        p {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 14px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 20px;
        }
        blockquote {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 21px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 30px;
        }
        pre {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 13px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 18.5714px;
        }

        .tags
        {
	        position: absolute;
            left: -9999px;
        }
        .js-sns-icon-container
        {
	        position: absolute;
            left: -9999px;
        }

        .news-info-block
        {
	        width: 100%;
	        background-color: #0d0d0d;
        }

        .information
        {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 12px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	
        }</style>";
#endregion

        public MalArticlesPage()
        {
            this.InitializeComponent();
            Loaded += (sedner,args) => ViewModel.Init();
            ViewModel.OpenWebView += ViewModelOnOpenWebView;
        }

        private void ViewModelOnOpenWebView(string html)
        {
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            var color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight1);
            var color1 = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentDark1);
            var color2 = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2);
            var css = Css.Replace("AccentColourBase", "#" + color2.ToString().Substring(3)).
                Replace("AccentColourLight", "#" + color.ToString().Substring(3)).
                Replace("AccentColourDark", "#" + color1.ToString().Substring(3));
            ArticleWebView.NavigateToString(css + Begin + html + "</div></body></html>");
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.LoadArticleCommand.Execute(e.ClickedItem);
        }

        private void ArticleWebView_OnScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value == "MouseBackNav")
            {
                NavMgr.CurrentMainViewOnBackRequested();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ArticleWebView.NavigateToString("");
            base.OnNavigatedFrom(e);
        }

        private void ArticleWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            try
            {
                if (args.Uri != null)
                {
                  if (args.Uri.ToString().Contains("anime/") || args.Uri.ToString().Contains("manga/"))
                    {
                        var link = args.Uri.ToString().Substring(7).Split('/');
                        ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                            new AnimeDetailsPageNavigationArgs(int.Parse(link[2]), link[3], null, null)
                            {
                                AnimeMode = link[1] == "anime"
                            });
                    }
                    args.Cancel = true;
                }
            }
            catch (Exception)
            {
                args.Cancel = true;
            }

        }

        private void ArticleWebView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            ViewModel.LoadingVisibility = Visibility.Collapsed;
            ViewModel.WebViewVisibility = Visibility.Visible;
        }
    }
}
