using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm.MagicalRawQueries.Messages;
using MALClient.Models;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class MalMessagingViewModel : ViewModelBase
    {
        public SmartObservableCollection<MalMessageModel> MessageIndex { get; } = new SmartObservableCollection<MalMessageModel>();

        private int _loadedPages = 1;
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

        private Visibility _loadMorePagesVisibility = Visibility.Collapsed;

        public Visibility LoadMorePagesVisibility
        {
            get { return _loadMorePagesVisibility; }
            set
            {
                _loadMorePagesVisibility = value;
                RaisePropertyChanged(() => LoadMorePagesVisibility);
            }
        }

        private ICommand _loadMoreCommand;

        public ICommand LoadMoreCommand => _loadMoreCommand ?? (_loadMoreCommand = new RelayCommand(LoadMore));

        public void Init()
        {
            LoadMore();
        }

        private async void LoadMore()
        {
            LoadingVisibility = Visibility.Visible;
            try
            {
                MessageIndex.AddRange(await AccountMessagesManager.GetMessagesAsync(_loadedPages++));
                LoadMorePagesVisibility = Visibility.Visible;
            }
            catch (ArgumentOutOfRangeException)
            {
                LoadMorePagesVisibility = Visibility.Collapsed;
            }

            LoadingVisibility = Visibility.Collapsed;
        }

    }
}
