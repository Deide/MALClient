using System;
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

        public async void Init()
        {
            Articles = await new MalArticlesIndexQuery().GetArticlesIndex();
        }

        public async void LoadArticle(MalNewsUnitModel data)
        {
            WebViewVisibility = Visibility.Visible;
            NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => WebViewVisibility = Visibility.Collapsed));
            OpenWebView?.Invoke(await new MalArticleQuery(data.Url).GetArticleHtml());
        }
    }
}
