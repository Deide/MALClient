﻿using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.UserControls;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public enum AnimeListWorkModes
    {
        Anime,
        SeasonalAnime,
        Manga,
        TopAnime,
        TopManga
    }

    public class AnimeListPageNavigationArgs
    {
        public readonly int CurrPage;
        public readonly bool Descending;
        public readonly string ListSource;
        public readonly bool NavArgs;
        public readonly int Status;
        public readonly int? StatusIndex;
        public AnimeSeason CurrSeason;
        public AnimeListDisplayModes DisplayMode;
        public SortOptions SortOption;
        public AnimeListWorkModes WorkMode = AnimeListWorkModes.Anime;

        public AnimeListPageNavigationArgs(SortOptions sort, int status, bool desc, int page,
            AnimeListWorkModes seasonal, string source, AnimeSeason season, AnimeListDisplayModes dispMode)
        {
            SortOption = sort;
            Status = status;
            Descending = desc;
            CurrPage = page;
            WorkMode = seasonal;
            ListSource = source;
            NavArgs = true;
            CurrSeason = season;
            DisplayMode = dispMode;
        }

        private AnimeListPageNavigationArgs()
        {
        }

        public AnimeListPageNavigationArgs(int index, AnimeListWorkModes workMode)
        {
            WorkMode = workMode;
            StatusIndex = index;
        }

        public static AnimeListPageNavigationArgs Seasonal
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.SeasonalAnime};

        public static AnimeListPageNavigationArgs Manga
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.Manga};

        public static AnimeListPageNavigationArgs TopAnime
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.TopAnime};

        public static AnimeListPageNavigationArgs TopManga
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.TopManga};
    }

    public enum SortOptions
    {
        SortTitle,
        SortScore,
        SortWatched,
        SortAirDay,
        SortNothing
    }

    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimeListPage : Page
    {
        private ScrollViewer _indefiniteScrollViewer;
        public AnimeListViewModel ViewModel => DataContext as AnimeListViewModel;

        public ScrollViewer IndefiniteScrollViewer
        {
            get
            {
                return _indefiniteScrollViewer ??
                       (_indefiniteScrollViewer =
                           VisualTreeHelper.GetChild(
                               VisualTreeHelper.GetChild((DependencyObject) GetScrollingContainer(), 0), 0) as
                               ScrollViewer);
            }
            set { _indefiniteScrollViewer = value; }
        }

        public Flyout FlyoutViews => ViewsFlyout;
        public Flyout FlyoutFilters => FiltersFlyout;
        public Flyout FlyoutSorting => SortingFlyout;

        public GridView ListGridView => AnimesItemsIndefinite;
        public GridView GridView => AnimesGridIndefinite;

        public void FlyoutSeasonSelectionHide()
        {
            FlyoutSeasonSelection.Hide();
        }

        private void AnimesPivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            if (ViewModel.CanLoadPages)
                (args.Item.Content as AnimePagePivotContent).LoadContent();
        }


        private void AnimesPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.RemovedItems.Count > 0)
                    ((e.RemovedItems.First() as PivotItem).Content as AnimePagePivotContent).ResetSelection();
            }
            catch (Exception)
            {
                //
            }
        }

        private object GetScrollingContainer()
        {
            switch (ViewModel.DisplayMode)
            {
                case AnimeListDisplayModes.IndefiniteList:
                    return AnimesItemsIndefinite;
                case AnimeListDisplayModes.IndefiniteGrid:
                    return AnimesGridIndefinite;
                case AnimeListDisplayModes.IndefiniteCompactList:
                    return AnimeCompactItemsIndefinite;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Init

        private double _prevWidth;
        private double _prevHeight;

        public AnimeListPage()
        {
            InitializeComponent();
            ViewModel.View = this;
            Loaded += (sender, args) => ViewModel.CanAddScrollHandler = true;
            SizeChanged += (sender, args) =>
            {
                if (Math.Abs(args.NewSize.Height - _prevHeight) > 100 && Math.Abs(args.NewSize.Width - _prevWidth) > 100)
                {
                    ViewModelLocator.Main.View.InitSplitter();
                    _prevHeight = args.NewSize.Height;
                    _prevWidth = args.NewSize.Width;
                    if ((DataContext as AnimeListViewModel).AreThereItemsWaitingForLoad)
                    {
                        ViewModelLocator.AnimeList.RefreshList();
                    }
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init(e.Parameter as AnimeListPageNavigationArgs);
        }

        #endregion

        #region UIHelpers

        //internal void ScrollTo(AnimeItem animeItem)
        //{
        //    try
        //    {
        //        var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
        //        var offset = ViewModel._animeItems.TakeWhile(t => animeItem != t).Sum(t => t.ActualHeight);
        //        scrollViewer.ScrollToVerticalOffset(offset);
        //    }
        //    catch (Exception)
        //    {
        //        // ehh
        //    }
        //}

        #endregion

        #region ActionHandlersPin

        private void SelectSortMode(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            switch (btn.Text)
            {
                case "Title":
                    ViewModel.SortOption = SortOptions.SortTitle;
                    break;
                case "Score":
                    ViewModel.SortOption = SortOptions.SortScore;
                    break;
                case "Watched":
                case "Read":
                    ViewModel.SortOption = SortOptions.SortWatched;
                    break;
                case "Soonest airing":
                    ViewModel.SortOption = SortOptions.SortAirDay;
                    break;
                default:
                    ViewModel.SortOption = SortOptions.SortNothing;
                    break;
            }
            foreach (var child in SortToggles.Children)
            {
                (child as ToggleMenuFlyoutItem).IsChecked = false;
            }
            btn.IsChecked = true;
            ViewModel.RefreshList();
        }


        private void ChangeSortOrder(object sender, RoutedEventArgs e)
        {
            var chbox = sender as ToggleMenuFlyoutItem;
            ViewModel.SortDescending = chbox.IsChecked;
            ViewModel.RefreshList();
        }

        private async void ListSource_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((sender == null && e == null) || e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                TxtListSource.IsEnabled = false; //reset input
                TxtListSource.IsEnabled = true;
                FlyoutListSource.Hide();
                BottomCommandBar.IsOpen = false;
                await ViewModel.FetchData();
            }
        }

        private void ShowListSourceFlyout(object sender, RoutedEventArgs e)
        {
            FlyoutListSource.ShowAt(sender as FrameworkElement);
        }

        private void SetListSource(object sender, RoutedEventArgs e)
        {
            ListSource_OnKeyDown(null, null);
        }

        private void FlyoutListSource_OnOpened(object sender, object e)
        {
            TxtListSource.SelectAll();
        }

        internal void InitSortOptions(SortOptions option, bool descending)
        {
            switch (option)
            {
                case SortOptions.SortTitle:
                    SortTitle.IsChecked = true;
                    break;
                case SortOptions.SortScore:
                    SortScore.IsChecked = true;
                    break;
                case SortOptions.SortWatched:
                    Sort3.IsChecked = true;
                    break;
                case SortOptions.SortAirDay:
                    SortAiring.IsChecked = true;
                    break;
                case SortOptions.SortNothing:
                    SortNone.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
            BtnOrderDescending.IsChecked = descending;
        }

        #endregion
    }
}