﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.Models.Favourites;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class ProfilePageNavigationArgs
    {
        public string TargetUser { get; set; }
    }

    public sealed class ProfilePageViewModel : ViewModelBase
    {
        private List<int> _animeChartValues = new List<int>();

        private int _currentlySelectedInnerPivotIndex;

        private PivotItem _currentlySelectedInnerPivotItem;

        private int _currentlySelectedOuterPivotIndex;


        private PivotItem _currentlySelectedOuterPivotItem;
        private bool _dataLoaded;

        #region Props
        private Visibility _emptyFavAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavCharactersNoticeVisibility = Visibility.Collapsed;
        private Visibility _emptyFavMangaNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavPeopleNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentMangaNoticeVisibility = Visibility.Collapsed;
        private List<AnimeItemViewModel> _favAnime;
        private List<AnimeItemViewModel> _favManga;

        private bool _initialized;


        private Visibility _loadingVisibility = Visibility.Collapsed;

        private List<int> _mangaChartValues = new List<int>();

        private ICommand _navigateCharPageCommand;

        private ICommand _navigateDetailsCommand;

        private ICommand _navigatePersonPageCommand;

        private ICommand _navAnimeListCommand;
        private ICommand _navMangaListCommand;

        private List<AnimeItemViewModel> _recentAnime;
        private List<AnimeItemViewModel> _recentManga;

        public ProfilePageViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            MaxWidth = bounds.Width / 2.2;
        }

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

        public AnimeItemViewModel TemporarilySelectedAnimeItem
        {
            get { return null; }
            set { value?.NavigateDetails(PageIndex.PageProfile); }
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

        public ICommand NavigateAnimeListCommand
            =>
                _navAnimeListCommand ??
                (_navAnimeListCommand = new RelayCommand(() => ViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,new AnimeListPageNavigationArgs(0,AnimeListWorkModes.Anime) { ListSource = _currUser})));

        public ICommand NavigateMangaListCommand
            =>
                _navMangaListCommand ??
                (_navMangaListCommand = new RelayCommand(() => ViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,new AnimeListPageNavigationArgs(0,AnimeListWorkModes.Manga) { ListSource = _currUser})));

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

        private Visibility _loadingOhersLibrariesProgressVisiblity = Visibility.Collapsed;
        public Visibility LoadingOhersLibrariesProgressVisiblity
        {
            get { return _loadingOhersLibrariesProgressVisiblity; }
            set
            {
                _loadingOhersLibrariesProgressVisiblity = value;
                RaisePropertyChanged(() => LoadingOhersLibrariesProgressVisiblity);
            }
        }

        #endregion

        //anime -<>- manga
        private readonly Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>> _othersAbstractions =
            new Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>>();

        private string _currUser;
        public ProfilePageNavigationArgs PrevArgs;
        public async void LoadProfileData(ProfilePageNavigationArgs args, bool force = false)
        {
            if (args == null)
                args = PrevArgs;
            else
                PrevArgs = args;

            if(args == null)
                return;
            if (_currUser == null || _currUser != args.TargetUser || force)
            {
                LoadingVisibility = Visibility.Visible;
                await Task.Run(async () => CurrentData = await new ProfileQuery(false,args?.TargetUser ?? "").GetProfileData(force));
                _currUser = args?.TargetUser ?? Credentials.UserName;
            }
            FavAnime = new List<AnimeItemViewModel>();
            FavManga = new List<AnimeItemViewModel>();
            RecentManga = new List<AnimeItemViewModel>();
            RecentAnime = new List<AnimeItemViewModel>();
            ViewModelLocator.Main.CurrentStatus = $"{_currUser} - Profile";
            bool authenticatedUser = args == null || args.TargetUser == Credentials.UserName;
            RaisePropertyChanged(() => CurrentData);
            LoadingVisibility = Visibility.Collapsed;
            if (authenticatedUser)
            {
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
            }
            else
            {
                if (!_othersAbstractions.ContainsKey(args?.TargetUser ?? ""))
                {
                    LoadingOhersLibrariesProgressVisiblity = Visibility.Visible;
                    var data = new List<ILibraryData>();
                    await Task.Run( async () => data = await new LibraryListQuery(args.TargetUser, AnimeListWorkModes.Anime).GetLibrary(false));

                    var abstractions = new List<AnimeItemAbstraction>();
                    foreach (var libraryData in data.Where(entry => CurrentData.FavouriteAnime.Any(i => i == entry.Id) || CurrentData.RecentAnime.Any(i => i == entry.Id)))
                        abstractions.Add(new AnimeItemAbstraction(false,libraryData as AnimeLibraryItemData));

                    await Task.Run(async () => data = await new LibraryListQuery(args.TargetUser, AnimeListWorkModes.Manga).GetLibrary(false));

                    var mangaAbstractions = new List<AnimeItemAbstraction>();
                    foreach (var libraryData in data.Where(entry => CurrentData.FavouriteManga.Any(i => i == entry.Id) || CurrentData.RecentManga.Any(i => i == entry.Id)))
                        mangaAbstractions.Add(new AnimeItemAbstraction(false,libraryData as MangaLibraryItemData));

                    _othersAbstractions.Add(args.TargetUser,new Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>(abstractions,mangaAbstractions));

                    LoadingOhersLibrariesProgressVisiblity = Visibility.Collapsed;
                }

                var source = _othersAbstractions[args.TargetUser];
                var list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.FavouriteAnime)
                {
                    var data = source.Item1.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }

                }
                FavAnime = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.FavouriteManga)
                {
                    var data = source.Item2.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }

                }
                FavManga = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.RecentAnime)
                {
                    var data = source.Item1.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }

                }
                RecentAnime = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.RecentManga)
                {
                    var data = source.Item2.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }

                }
                RecentManga = list;

            }            
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

        }

        private void NavigateDetails(FavCharacter character)
        {
            ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.OriginatingShowName, null,
                    null)
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