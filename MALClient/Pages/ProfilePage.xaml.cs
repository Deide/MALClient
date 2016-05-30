﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Models;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        private static ProfilePageNavigationArgs _lastArgs;

        public ProfilePage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            (DataContext as ProfilePageViewModel).LoadProfileData(_lastArgs);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as ProfilePageNavigationArgs;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (DataContext as ProfilePageViewModel).Cleanup();
            base.OnNavigatedFrom(e);
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.Main.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = (e.ClickedItem as MalUser).Name });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.Main.Navigate(PageIndex.PageProfile,new ProfilePageNavigationArgs {TargetUser = (string)(sender as Button).Tag});
        }


        private void AnimesGridIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeGridItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }
    }
}