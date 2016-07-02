﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;

namespace MALClient.Models
{
    public class MalMessageModel : ViewModelBase
    {
        public string Sender { get; set; }
        private string _target;

        public string Target
        {
            get { return IsMine ? _target : Sender; }
            set { _target = value; }
        }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public string ThreadId { get; set; }
        public string ReplyId { get; set; }
        private bool _isRead { get; set; }
        public bool IsMine { get; set; }
        public Symbol Icon => IsMine ? Symbol.MailForward : IsRead ? Symbol.Read : Symbol.Mail;

        public bool IsRead
        {
            get { return _isRead; }
            set
            {
                _isRead = value;
                RaisePropertyChanged(() => Icon);
            }
        }
    }
}
