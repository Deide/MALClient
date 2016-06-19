using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
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
            public CornerRadius CornerRadius { get; set; }
            public Brush Background { get; set; }


            public MessageEntry(MalMessageModel msg)
            {
                Msg = msg;
                if (Msg.Sender.Equals(Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                {
                    HorizontalAlignment = HorizontalAlignment.Right;
                    Margin = new Thickness(20,0,0,0);
                    CornerRadius = new CornerRadius(10,10,0,10);
                    Background = Application.Current.Resources["SystemControlHighlightAltListAccentLowBrush"] as Brush;
                }
                else
                {
                    HorizontalAlignment = HorizontalAlignment.Left;
                    Margin = new Thickness(0, 0, 20, 0);
                    CornerRadius = new CornerRadius(10, 10, 10, 0);
                    Background = Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] as Brush;
                }
            }
        }

        private ICommand _fetchHistoryCommand;

        public ICommand FetchHistoryCommand
            => _fetchHistoryCommand ?? (_fetchHistoryCommand = new RelayCommand(FetchHistory));

        private Visibility _fetchHistoryVisibility;

        public Visibility FetchHistoryVisibility
        {
            get { return _fetchHistoryVisibility; }
            set
            {
                _fetchHistoryVisibility = value;
                RaisePropertyChanged(() => FetchHistoryVisibility);
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

        private ICommand _sendMessageCommand { get; set; }

        public ICommand SendMessageCommand
            => _sendMessageCommand ?? (_sendMessageCommand = new RelayCommand(SendMessage));
        public string MessageText { get; set; }

        public SmartObservableCollection<MessageEntry> MessageSet { get; } =
            new SmartObservableCollection<MessageEntry>();

        private MalMessageModel _prevMsg;

        public async void Init(MalMessageModel args)
        {
            if(_prevMsg?.Id == args.Id)
                return;
       
            MessageSet.Clear();
            LoadingVisibility = Visibility.Visible;
            args = await new MalMessageDetailsQuery().GetMessageDetails(args);
            _prevMsg = args;
            if (_messageThreads.ContainsKey(args.ThreadId))
            {
                FetchHistoryVisibility = Visibility.Collapsed;
                MessageSet.AddRange(_messageThreads[args.ThreadId].Select(model => new MessageEntry(model)));
            }
            else
            {
                MessageSet.AddRange(new MessageEntry[] {new MessageEntry(args)});
                FetchHistoryVisibility = Visibility.Visible;
            }
            LoadingVisibility = Visibility.Collapsed;
        }

        private async void FetchHistory()
        {
            LoadingVisibility = Visibility.Visible;
            FetchHistoryVisibility = Visibility.Collapsed;
            MessageSet.Clear();
            var result = await new MalMessageDetailsQuery().GetMessagesInThread(_prevMsg);
            result.Reverse(); //newest first
            _messageThreads[_prevMsg.ThreadId] = result;
            MessageSet.AddRange(result.Select(model => new MessageEntry(model)));
            LoadingVisibility = Visibility.Collapsed;
        }

        private async void SendMessage()
        {
            bool sent = await new SendMessageQuery().SendMessage(_prevMsg.Subject, MessageText, _prevMsg.Sender, _prevMsg.ThreadId, _prevMsg.ReplyId);
            if (sent)
            {
                var message = new MalMessageModel
                {
                    Subject = _prevMsg.Subject,
                    Content = MessageText,
                    Date = "-",
                    Id = "0",
                    Sender = _prevMsg.Sender,
                    ThreadId = _prevMsg.ThreadId,

                };
                if (_messageThreads.ContainsKey(_prevMsg.ThreadId))
                {
                    _messageThreads[_prevMsg.ThreadId].Insert(0, message);
                }
                else
                {
                    _messageThreads[_prevMsg.ThreadId] = new List<MalMessageModel> {_prevMsg,message};
                }
                MessageSet.AddRange(new MessageEntry[]
                {
                    new MessageEntry(message)
                });
            }
        }

    }
}
