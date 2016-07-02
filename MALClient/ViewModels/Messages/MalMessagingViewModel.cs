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
        public List<MalMessageModel> Outbox { get; set; } = new List<MalMessageModel>();
        public List<MalMessageModel> Inbox { get; set; } = new List<MalMessageModel>();

        private bool _skipLoading;
        private int _loadedPages = 1;
        private int _selectedMessageIndex = -1;

        public int SelectedMessageIndex
        {
            get { return _selectedMessageIndex; }
            set
            {
                if(MessageIndex[value].IsMine)
                    return;
                _selectedMessageIndex = value;
                ViewModelLocator.Main.Navigate(PageIndex.PageMessageDetails,MessageIndex[value]);
                RaisePropertyChanged(() => SelectedMessageIndex);
            }
        }

        private bool _displaySentMessages;
        public bool DisplaySentMessages
        {
            get { return _displaySentMessages; }
            set
            {
                _displaySentMessages = value;
                _skipLoading = true;
                LoadMore();
                RaisePropertyChanged(() => DisplaySentMessages);
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

        public ICommand LoadMoreCommand => _loadMoreCommand ?? (_loadMoreCommand = new RelayCommand( () => LoadMore()));

        private ICommand _composeNewCommand;

        public ICommand ComposeNewCommand => _composeNewCommand ?? (_composeNewCommand = new RelayCommand(ComposeNew));


        public void Init(bool force = false)
        {
            LoadMore(force);
        }

        private async void LoadMore(bool force = false)
        {
            LoadingVisibility = Visibility.Visible;
            if (force)
            {
                if (DisplaySentMessages)
                {
                    Outbox = new List<MalMessageModel>();
                }
                else
                {
                    _loadedPages = 1;
                    Inbox = new List<MalMessageModel>();
                }
            }
            if (!DisplaySentMessages)
                try
                {
                    if(!_skipLoading)
                        Inbox.AddRange(await AccountMessagesManager.GetMessagesAsync(_loadedPages++));
                    _skipLoading = false;
                    MessageIndex.Clear();
                    MessageIndex.AddRange(Inbox);
                    LoadMorePagesVisibility = Visibility.Visible;
                }
                catch (ArgumentOutOfRangeException)
                {
                    LoadMorePagesVisibility = Visibility.Collapsed;
                }
            else
                try
                {
                    if(Outbox.Count == 0)
                        Outbox = await AccountMessagesManager.GetSentMessagesAsync();
                    MessageIndex.Clear();
                    MessageIndex.AddRange(Outbox);
                    LoadMorePagesVisibility = Visibility.Collapsed;
                }
                catch (Exception)
                {
                    
                    throw;
                }
 
            LoadingVisibility = Visibility.Collapsed;
        }

        private void ComposeNew()
        {
            ViewModelLocator.Main.Navigate(PageIndex.PageMessageDetails, null); // null for new message
        }

    }
}
