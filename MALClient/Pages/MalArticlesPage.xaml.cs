using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps.LocalSearch;
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

        const string JsBackNavFunction = @"<script>function PointerPressed(event) {var x = event.buttons;window.external.notify(""x.buttons"")}</script><html><body><div onmousedown=""PointerPressed(event)"">";

        public MalArticlesPage()
        {
            this.InitializeComponent();
            Loaded += (sedner,args) => ViewModel.Init();
            ViewModel.OpenWebView += ViewModelOnOpenWebView;
        }

        private void ViewModelOnOpenWebView(string html)
        {          
            ArticleWebView.NavigateToString(JsBackNavFunction + html + "</div></body></html>");
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
    }
}
