﻿using System;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.ViewModels;

#pragma warning disable 4014

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public enum HamburgerButtons
    {
        AnimeList,
        AnimeSearch,
        LogIn,
        Settings,
        Profile,
        Seasonal,
        About,
        Recommendations,
        MangaList,
        MangaSearch,
        TopAnime,
        TopManga,
        Calendar,
        Articles,
        News
    }

    public sealed partial class HamburgerControl : UserControl
    {
        private static readonly Brush _b2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(220, 50, 50, 50)
                : Color.FromArgb(220, 190, 190, 190));

        private bool _animeFiltersExpanded;
        private bool _mangaFiltersExpanded;
        private bool _topCategoriesExpanded;

        public HamburgerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            if (Settings.EnableHearthAnimation)
                SupportMeStoryboard.Begin();
        }

        private HamburgerControlViewModel ViewModel => (HamburgerControlViewModel) DataContext;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.UpdateProfileImg();
            if (Settings.HamburgerAnimeFiltersExpanded)
                ButtonExpandAnimeFiltersOnClick(null, null);
            if (Settings.HamburgerMangaFiltersExpanded)
                ButtonExpandMangaFiltersOnClick(null, null);
            if (Settings.HamburgerTopCategoriesExpanded)
                ButtonExpandTopCategoriesOnClick(null, null);

            FeedbackImage.Source = Settings.SelectedTheme == ApplicationTheme.Dark
                ? new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-Light-120px-plus.png"))
                : new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-120px-plus.png"));
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = _b2;
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ButtonExpandAnimeFiltersOnRightClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.HamburgerExpanded)
                ButtonExpandAnimeFiltersOnClick(null, null);
            else
                HamburgerFlyoutService.ShowAnimeFiltersFlyout(sender as FrameworkElement);
        }
        private void ButtonExpandAnimeFiltersOnClick(object sender, RoutedEventArgs e)
        {           
            if (!_animeFiltersExpanded)
            {
                ExpandAnimeListFiltersStoryboard.Begin();
                RotateAnimeListFiltersStoryboard.Begin();
            }
            else
            {
                CollapseAnimeListFiltersStoryboard.Begin();
                RotateBackAnimeListFiltersStoryboard.Begin();
            }

            _animeFiltersExpanded = !_animeFiltersExpanded;
        }

        private void ButtonExpandMangaFiltersOnRightClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.HamburgerExpanded)
                ButtonExpandMangaFiltersOnClick(null, null);
            else
                HamburgerFlyoutService.ShowAnimeMangaFiltersFlyout(sender as FrameworkElement);
        }

        private void ButtonExpandMangaFiltersOnClick(object sender, RoutedEventArgs e)
        {
            if (!_mangaFiltersExpanded)
            {
                ExpandMangaListFiltersStoryboard.Begin();
                RotateMangaListFiltersStoryboard.Begin();
            }
            else
            {
                CollapseMangaListFiltersStoryboard.Begin();
                RotateBackMangaListFiltersStoryboard.Begin();
            }

            _mangaFiltersExpanded = !_mangaFiltersExpanded;
        }


        private void ButtonExpandTopCategoriesOnRightClick(object sender, RoutedEventArgs e)
        {
            if(ViewModel.HamburgerExpanded)
                ButtonExpandTopCategoriesOnClick(null,null);
            else
                HamburgerFlyoutService.ShowTopCategoriesFlyout(sender as FrameworkElement);
        }

        private void ButtonExpandTopCategoriesOnClick(object sender, RoutedEventArgs e)
        {
            if (!_topCategoriesExpanded)
            {
                ExpandTopAnimeCategoriesStoryboard.Begin();
                RotateTopAnimeCategoriesStoryboard.Begin();
            }
            else
            {
                CollapseTopAnimeCategoriesStoryboard.Begin();
                RotateBackTopAnimeCategoriesStoryboard.Begin();
            }

            _topCategoriesExpanded = !_topCategoriesExpanded;
        }

        private void HamburgerControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == 250.0) //opened
            {
                ViewModel.HamburgerWidthChanged(true);
                MidSeparator.Width = BottomSeparator.Width = 250;
                Mid2Separator.Width = BottomSeparator.Width = 250;
            }
            else //closed
            {
                ViewModel.HamburgerWidthChanged(false);
                MidSeparator.Width = BottomSeparator.Width = 60;
                Mid2Separator.Width = BottomSeparator.Width = 60;
                if (_topCategoriesExpanded)
                {
                    CollapseTopAnimeCategoriesStoryboard.Begin();
                    RotateBackTopAnimeCategoriesStoryboard.Begin();
                    _topCategoriesExpanded = false;
                }
                if (_animeFiltersExpanded)
                {
                    CollapseAnimeListFiltersStoryboard.Begin();
                    RotateBackAnimeListFiltersStoryboard.Begin();
                    _animeFiltersExpanded = false;
                }
                if (_mangaFiltersExpanded)
                {
                    CollapseMangaListFiltersStoryboard.Begin();
                    RotateBackMangaListFiltersStoryboard.Begin();
                    _mangaFiltersExpanded = false;
                }
            }
        }

       

        private async void Donate(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as MenuFlyoutItem;
                await CurrentApp.RequestProductPurchaseAsync(btn.Tag as string, false);
                Settings.Donated = true;
            }
            catch (Exception)
            {
                // no donation
            }
        }

        private async void OpenRepo(object sender, RoutedEventArgs e)
        {
            Utils.TelemetryTrackEvent(TelemetryTrackedEvents.LaunchedFeedback);
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient/issues"));
        }

    }
}