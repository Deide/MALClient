using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Comm.MagicalRawQueries.Messages;
using MALClient.Models;

namespace MALClient.ViewModels
{
    public class MalMessagingViewModel : ViewModelBase
    {
        public SmartObservableCollection<MalMessageModel> MessageIndex { get; } = new SmartObservableCollection<MalMessageModel>();

        private int _loadedPages = 0;
        private bool _allPagesLoaded;

        public async void Init()
        {
            if (_loadedPages == 0)
                MessageIndex.AddRange(await AccountMessagesManager.GetMessagesAsync(0));
        }

    }
}
