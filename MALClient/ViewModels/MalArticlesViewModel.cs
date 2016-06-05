using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Models;

namespace MALClient.ViewModels
{
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

        public async void Init()
        {
            Articles = await new MalArticlesIndexQuery().GetArticlesIndex();
        }
    }
}
