using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MALClient.Models
{
    public class MalMessageModel : ViewModelBase
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public string ThreadId { get; set; }
        public string ReplyId { get; set; }
        private bool _isRead { get; set; }

        public bool IsRead
        {
            get { return _isRead; }
            set
            {
                _isRead = value;
                RaisePropertyChanged(() => IsRead);
            }
        }
    }
}
