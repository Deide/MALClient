using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Models;
using MALClient.Models.Favourites;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class ProfilePageNavigationArgs
    {
        public int OuterPivotSelectedIndex { get; set; }
        public int InnerPivotSelectedIndex { get; set; }
    }

    public sealed class ProfilePageViewModel : ViewModelBase
    {
        private List<int> _animeChartValues = new List<int>();

        private int _currentlySelectedInnerPivotIndex;

        private PivotItem _currentlySelectedInnerPivotItem;

        private int _currentlySelectedOuterPivotIndex;


        private PivotItem _currentlySelectedOuterPivotItem;
        private bool _dataLoaded;


        private Visibility _emptyFavAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavCharactersNoticeVisibility = Visibility.Collapsed;
        private Visibility _emptyFavMangaNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavPeopleNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentMangaNoticeVisibility = Visibility.Collapsed;
        private List<AnimeItemViewModel> _favAnime;
        private List<FavCharacter> _favChars;
        private List<AnimeItemViewModel> _favManga;
        private List<FavPerson> _favPpl;


        private bool _initialized;
        private bool _loadedChars;
        private bool _loadedFavAnime;
        private bool _loadedFavManga;
        private bool _loadedPpl;
        private bool _loadedRecent;
        private bool _loadedStats;

        private Visibility _loadingVisibility = Visibility.Collapsed;

        private List<int> _mangaChartValues = new List<int>();

        private ICommand _navigateCharPageCommand;

        private ICommand _navigateDetailsCommand;

        private ICommand _navigatePersonPageCommand;

        private List<AnimeItemViewModel> _recentAnime;
        private List<AnimeItemViewModel> _recentManga;

        public ProfilePageViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            MaxWidth = bounds.Width/2.2;
        }

        //public ProfilePage View { get; set; }

        public ProfileData CurrentData { get; set; } = new ProfileData();

        public List<AnimeItemViewModel> RecentAnime
        {
            get { return _recentAnime; }
            private set
            {
                _recentAnime = value;
                RaisePropertyChanged(() => RecentAnime);
            }
        }

        public List<AnimeItemViewModel> RecentManga
        {
            get { return _recentManga; }
            private set
            {
                _recentManga = value;
                RaisePropertyChanged(() => RecentManga);
            }
        }

        public List<AnimeItemViewModel> FavAnime
        {
            get { return _favAnime; }
            private set
            {
                _favAnime = value;
                RaisePropertyChanged(() => FavAnime);
            }
        }

        public List<AnimeItemViewModel> FavManga
        {
            get { return _favManga; }
            private set
            {
                _favManga = value;
                RaisePropertyChanged(() => FavManga);
            }
        }

        public List<FavCharacter> FavCharacters
        {
            get { return _favChars; }
            private set
            {
                _favChars = value;
                RaisePropertyChanged(() => FavCharacters);
            }
        }

        public List<FavPerson> FavPeople
        {
            get { return _favPpl; }
            private set
            {
                _favPpl = value;
                RaisePropertyChanged(() => FavPeople);
            }
        }

        public PivotItem CurrentlySelectedOuterPivotItem
        {
            get { return _currentlySelectedOuterPivotItem; }
            set
            {
                _currentlySelectedOuterPivotItem = value;
                //RaisePropertyChanged(() => CurrentlySelectedOuterPivotItem);
                OuterPivotItemChanged(value.Tag as string);
            }
        }

        public PivotItem CurrentlySelectedInnerPivotItem
        {
            get { return _currentlySelectedInnerPivotItem; }
            set
            {
                _currentlySelectedInnerPivotItem = value;
                //RaisePropertyChanged(() => CurrentlySelectedInnerPivotItem);
                InnerPivotItemChanged(value.Tag as string);
            }
        }

        public AnimeItemViewModel TemporarilySelectedAnimeItem
        {
            get { return null; }
            set
            {
                value?.NavigateDetails(PageIndex.PageProfile,
                    new ProfilePageNavigationArgs
                    {
                        InnerPivotSelectedIndex = CurrentlySelectedInnerPivotIndex,
                        OuterPivotSelectedIndex = CurrentlySelectedOuterPivotIndex
                    });
            }
        }

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        public int CurrentlySelectedOuterPivotIndex
        {
            get { return _currentlySelectedOuterPivotIndex; }
            set
            {
                _currentlySelectedOuterPivotIndex = value;
                RaisePropertyChanged(() => CurrentlySelectedOuterPivotIndex);
            }
        }

        public int CurrentlySelectedInnerPivotIndex
        {
            get { return _currentlySelectedInnerPivotIndex; }
            set
            {
                _currentlySelectedInnerPivotIndex = value;
                RaisePropertyChanged(() => CurrentlySelectedInnerPivotIndex);
            }
        }

        public List<int> AnimeChartValues
        {
            get { return _animeChartValues; }
            set
            {
                _animeChartValues = value;
                RaisePropertyChanged(() => AnimeChartValues);
            }
        }

        public List<int> MangaChartValues
        {
            get { return _mangaChartValues; }
            set
            {
                _mangaChartValues = value;
                RaisePropertyChanged(() => MangaChartValues);
            }
        }

        public static double MaxWidth { get; set; }

        public ICommand NavigateDetailsCommand
            => _navigateDetailsCommand ?? (_navigateDetailsCommand = new RelayCommand<FavCharacter>(NavigateDetails));

        public ICommand NavigateCharPageCommand
            =>
                _navigateCharPageCommand ??
                (_navigateCharPageCommand = new RelayCommand<FavCharacter>(NavigateCharacterWebPage));

        public ICommand NavigatePersonPageCommand
            =>
                _navigatePersonPageCommand ??
                (_navigatePersonPageCommand = new RelayCommand<FavPerson>(NavigatePersonWebPage));

        public Visibility EmptyFavAnimeNoticeVisibility
        {
            get { return _emptyFavAnimeNoticeVisibility; }
            set
            {
                _emptyFavAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavAnimeNoticeVisibility);
            }
        }

        public Visibility EmptyFavCharactersNoticeVisibility
        {
            get { return _emptyFavCharactersNoticeVisibility; }
            set
            {
                _emptyFavCharactersNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavCharactersNoticeVisibility);
            }
        }

        public Visibility EmptyFavMangaNoticeVisibility
        {
            get { return _emptyFavMangaNoticeVisibility; }
            set
            {
                _emptyFavMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavMangaNoticeVisibility);
            }
        }

        public Visibility EmptyRecentMangaNoticeVisibility
        {
            get { return _emptyRecentMangaNoticeVisibility; }
            set
            {
                _emptyRecentMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentMangaNoticeVisibility);
            }
        }

        public Visibility EmptyRecentAnimeNoticeVisibility
        {
            get { return _emptyRecentAnimeNoticeVisibility; }
            set
            {
                _emptyRecentAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentAnimeNoticeVisibility);
            }
        }

        public Visibility EmptyFavPeopleNoticeVisibility
        {
            get { return _emptyFavPeopleNoticeVisibility; }
            set
            {
                _emptyFavPeopleNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavPeopleNoticeVisibility);
            }
        }

        public async void LoadProfileData(ProfilePageNavigationArgs args, bool force = false)
        {
            if (!_dataLoaded || force)
            {
                LoadingVisibility = Visibility.Visible;
                Cleanup();
                await Task.Run(async () => CurrentData = await new ProfileQuery().GetProfileData(force));
                _dataLoaded = true;
            }
            RaisePropertyChanged(() => CurrentData);
            _initialized = true;
            var list = new List<AnimeItemViewModel>();
            foreach (var id in CurrentData.FavouriteAnime)
            {
                var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                
                if (data != null)
                {
                    list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                }
                
            }
            FavAnime = list;
            list = new List<AnimeItemViewModel>();
            foreach (var id in CurrentData.FavouriteManga)
            {
                var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                if (data != null)
                {
                    list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                }

            }
            FavManga = list;
            list = new List<AnimeItemViewModel>();
            foreach (var id in CurrentData.RecentAnime)
            {
                var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                if (data != null)
                {
                    list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                }

            }
            RecentAnime = list;
            list = new List<AnimeItemViewModel>();
            foreach (var id in CurrentData.RecentManga)
            {
                var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                if (data != null)
                {
                    list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                }

            }
            RecentManga = list;
            EmptyRecentAnimeNoticeVisibility = RecentAnime.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyRecentMangaNoticeVisibility = RecentManga.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavCharactersNoticeVisibility = CurrentData.FavouriteCharacters.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavAnimeNoticeVisibility = FavAnime.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavMangaNoticeVisibility = FavManga.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavPeopleNoticeVisibility = CurrentData.FavouritePeople.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            LoadingVisibility = Visibility.Collapsed;
        }

        private async void InnerPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;
            switch (tag)
            {
                case "Chars":
                    if (_loadedChars)
                        return;
                    _loadedChars = true;
                    await Task.Delay(10);


                    break;
                case "Anime":
                    if (_loadedFavAnime)
                        break;
                    _loadedFavAnime = true;

                    break;
                case "Manga":
                    if (_loadedFavManga)
                        break;
                    _loadedFavManga = true;


                    break;
                case "Ppl":
                    if (_loadedPpl)
                        return;
                    _loadedPpl = true;
                    await Task.Delay(10);
                    foreach (var favPerson in CurrentData.FavouritePeople)
                    {
                        favPerson.LoadBitmap();
                        FavPeople.Add(favPerson);
                    }

                    break;
            }
        }

        private async void OuterPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;
            switch (tag)
            {
                case "Favs":
                    InnerPivotItemChanged(CurrentlySelectedInnerPivotItem.Tag as string);
                    _loadedRecent = false;
                    break;
                case "Recent":
                    if (_loadedRecent)
                        break;
                    _loadedRecent = true;
                    //in case of duplicate we have to clear this
                    _loadedFavManga = false;
                    _loadedFavAnime = false;


                    break;
                case "Stats":
                    if (_loadedStats)
                        return;
                    _loadedStats = true;
                    AnimeChartValues = new List<int>
                    {
                        CurrentData.AnimeWatching,
                        CurrentData.AnimeCompleted,
                        CurrentData.AnimeOnHold,
                        CurrentData.AnimeDropped,
                        CurrentData.AnimePlanned
                    };
                    MangaChartValues = new List<int>
                    {
                        CurrentData.MangaReading,
                        CurrentData.MangaCompleted,
                        CurrentData.MangaOnHold,
                        CurrentData.MangaDropped,
                        CurrentData.MangaPlanned
                    };
                    break;
            }
        }

        private void NavigateDetails(FavCharacter character)
        {
            ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.OriginatingShowName, null,
                    null,
                    new ProfilePageNavigationArgs
                    {
                        InnerPivotSelectedIndex = CurrentlySelectedInnerPivotIndex,
                        OuterPivotSelectedIndex = CurrentlySelectedOuterPivotIndex
                    })
                {
                    Source = PageIndex.PageProfile,
                    AnimeMode = character.FromAnime
                });
        }

        private async void NavigateCharacterWebPage(FavCharacter character)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/character/{character.Id}"));
        }

        private async void NavigatePersonWebPage(FavPerson person)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/people/{person.Id}"));
        }

    }
}