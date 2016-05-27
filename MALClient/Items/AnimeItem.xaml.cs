using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl
    {
        public AnimeItem() //xaml constructor
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Bindings.Update();
        }

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        private void BtnWatchedEpsOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowWatchedEpisodesFlyout(sender as FrameworkElement);
        }

        private void BtnMoreOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemFlyout(sender as FrameworkElement);
        }

        private void BtnScoreOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemScoreFlyout(sender as FrameworkElement);
        }

        private void BtnStatusOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemStatusFlyout(sender as FrameworkElement);
        }
    }
}