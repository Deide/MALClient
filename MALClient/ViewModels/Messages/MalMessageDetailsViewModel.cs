using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm.MagicalRawQueries.Messages;
using MALClient.Models;

namespace MALClient.ViewModels.Messages
{
    public class MalMessageDetailsViewModel : MainViewModel
    {
        private static readonly Dictionary<string,List<MalMessageModel>> _messageThreads = new Dictionary<string, List<MalMessageModel>>();

        public class MessageEntry
        {
            public MalMessageModel Msg { get; set; }
            public HorizontalAlignment HorizontalAlignment { get; set; }
            public Thickness Margin { get; set; }

            public MessageEntry(MalMessageModel msg)
            {
                Msg = msg;
                if (Msg.Sender.Equals(Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                {
                    HorizontalAlignment = HorizontalAlignment.Right;
                    Margin = new Thickness(20,0,0,0);
                }
                else
                {
                    HorizontalAlignment = HorizontalAlignment.Left;
                    Margin = new Thickness(0, 0, 20, 0);
                }
            }
        }

        private ICommand _fetchHistoryCommand;

        public ICommand FetchHistoryCommand
            => _fetchHistoryCommand ?? (_fetchHistoryCommand = new RelayCommand(FetchHistory));

        private Visibility _fetchHistoryVisibility;

        private Visibility FetchHistoryVisibility
        {
            get { return _fetchHistoryVisibility; }
            set
            {
                _fetchHistoryVisibility = value;
                RaisePropertyChanged(() => FetchHistoryVisibility);
            }
        }

        private Visibility _loadingVisibility;

        private Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        public SmartObservableCollection<MessageEntry> MessageSet { get; } =
            new SmartObservableCollection<MessageEntry>();

        private MalMessageModel _prevMsg;

        public async void Init(MalMessageModel args)
        {
            if(_prevMsg?.Id == args.Id)
                return;
            _prevMsg = args;

        
            MessageSet.Clear();
            LoadingVisibility = Visibility.Visible;
            args = await new MalMessageDetailsQuery().GetMessageDetails(args);
            MessageSet.Add(new MessageEntry(args));
            if (_messageThreads.ContainsKey(args.Id))
            {
                FetchHistoryVisibility = Visibility.Collapsed;
                MessageSet.AddRange(_messageThreads[args.Id].Select(model => new MessageEntry(model)));
            }
            else
            {
                FetchHistoryVisibility = Visibility.Visible;
            }
        }

        private async void FetchHistory()
        {
            LoadingVisibility = Visibility.Visible;
            FetchHistoryVisibility = Visibility.Collapsed;
            var result = await new MalMessageDetailsQuery().GetMessagesInThread(_prevMsg);
            _messageThreads[_prevMsg.Id] = result;
            MessageSet.AddRange(result.Select(model => new MessageEntry(model)));
            LoadingVisibility = Visibility.Collapsed;
        }
    }
}
