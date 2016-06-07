﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Comm.Articles;
using MALClient.Models;

namespace MALClient.ViewModels
{
    public enum ArticlePageWorkMode
    {
        Articles,
        News
    }

    public class MalArticlesPageNavigationArgs
    {
        public ArticlePageWorkMode WorkMode { get; set; }

        private MalArticlesPageNavigationArgs()
        {
            
        }

        public static MalArticlesPageNavigationArgs Articles => new MalArticlesPageNavigationArgs {WorkMode = ArticlePageWorkMode.Articles};
        public static MalArticlesPageNavigationArgs News => new MalArticlesPageNavigationArgs {WorkMode = ArticlePageWorkMode.News};
    }

    public delegate void OpenWebViewRequest(string html);

    public class MalArticlesViewModel : ViewModelBase
    {
        private List<MalNewsUnitModel> _articles = new List<MalNewsUnitModel>();

        public List<MalNewsUnitModel> Articles
        {
            get { return _articles; }
            set
            {
                _articles = value;
                RaisePropertyChanged(() => Articles);
            }
        }

        public event OpenWebViewRequest OpenWebView;

        private ICommand _loadArticleCommand;

        public ICommand LoadArticleCommand
            => _loadArticleCommand ?? (_loadArticleCommand = new RelayCommand<MalNewsUnitModel>(LoadArticle));

        private Visibility _webViewVisibility = Visibility.Collapsed;

        public Visibility WebViewVisibility
        {
            get { return _webViewVisibility; }
            set
            {
                _webViewVisibility = value;
                RaisePropertyChanged(() => WebViewVisibility);
            }
        }

        private Visibility _articleIndexVisibility = Visibility.Visible;

        public Visibility ArticleIndexVisibility
        {
            get { return _articleIndexVisibility; }
            set
            {
                _articleIndexVisibility = value;
                RaisePropertyChanged(() => ArticleIndexVisibility);
            }
        }

        private Visibility _loadingVisibility = Visibility.Collapsed;

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        private double _thumbnailWidth = 200;
        private double _thumbnailHeight = 200;

        public double ThumbnailWidth
        {
            get { return _thumbnailWidth; }
            set
            {
                _thumbnailWidth = value;
                RaisePropertyChanged(() => ThumbnailWidth);
            }
        }

        public double ThumbnailHeight
        {
            get { return _thumbnailHeight; }
            set
            {
                _thumbnailHeight = value;
                RaisePropertyChanged(() => ThumbnailHeight);
            }
        }

        private ArticlePageWorkMode? _prevWorkMode;
        public async void Init(MalArticlesPageNavigationArgs args,bool force = false)
        {
            ArticleIndexVisibility = Visibility.Visible;
            WebViewVisibility = Visibility.Collapsed;
            ViewModelLocator.Main.CurrentStatus = args.WorkMode == ArticlePageWorkMode.Articles ? "Articles" : "News";

            if (_prevWorkMode == args?.WorkMode)
                return;



            switch (args.WorkMode)
            {
                case ArticlePageWorkMode.Articles:
                    ThumbnailWidth = ThumbnailHeight = 200;
                    break;
                case ArticlePageWorkMode.News:
                    ThumbnailWidth = 100;
                    ThumbnailHeight = 150;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _prevWorkMode = args.WorkMode;

            Articles.Clear();
            LoadingVisibility = Visibility.Visible;
            Articles = await Task.Run(async () => await new MalArticlesIndexQuery(args.WorkMode).GetArticlesIndex(force));
            LoadingVisibility = Visibility.Collapsed;
        }

        private async void LoadArticle(MalNewsUnitModel data)
        {
            LoadingVisibility = Visibility.Visible;
            ArticleIndexVisibility = Visibility.Collapsed;
            ViewModelLocator.Main.CurrentStatus = data.Title;
            NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() =>
            {
                WebViewVisibility = Visibility.Collapsed;
                ArticleIndexVisibility = Visibility.Visible;
                ViewModelLocator.Main.CurrentStatus = "Articles";
            }));
            OpenWebView?.Invoke(await new MalArticleQuery(data.Url, data.Title,data.Type).GetArticleHtml());
        }
    }
}
