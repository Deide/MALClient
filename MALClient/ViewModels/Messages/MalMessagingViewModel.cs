using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using MALClient.Comm.MagicalRawQueries.Messages;
using MALClient.Models;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class MalMessagingViewModel : ViewModelBase
    {
        public SmartObservableCollection<MalMessageModel> MessageIndex { get; } = new SmartObservableCollection<MalMessageModel>();

        private int _loadedPages = 0;
        private bool _allPagesLoaded;

        private int _selectedMessageIndex;

        public int SelectedMessageIndex
        {
            get { return _selectedMessageIndex; }
            set
            {
                _selectedMessageIndex = value;
                ViewModelLocator.Main.Navigate(PageIndex.PageMessageDetails,MessageIndex[value]);
                RaisePropertyChanged(() => SelectedMessageIndex);
            }
        }

        private Visibility _loadingVisibility;

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        public async void Init()
        {
            LoadingVisibility = Visibility.Visible;
            if (_loadedPages == 0)
                MessageIndex.AddRange(await AccountMessagesManager.GetMessagesAsync(1));
            LoadingVisibility = Visibility.Collapsed;
        }

    }
}
