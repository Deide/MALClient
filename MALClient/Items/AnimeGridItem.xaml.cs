﻿using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeGridItem : UserControl
    {
        public AnimeGridItem(AnimeItemViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        public AnimeGridItem()
        {
            InitializeComponent();
        }

        //public void ClearImage()
        //{
        //    Image.Source = null;
        //}

        //public void BindImage()
        //{
        //    if(Image.Source != null)
        //        return;
        //    var bnd = new Binding {Source = ViewModel.Image};
        //    Image.SetBinding(Image.SourceProperty,bnd);
        //}

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        private void BtnMoreClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeGridItemFlyout(sender as FrameworkElement);
        }

        private void WatchedFlyoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowWatchedEpisodesFlyout(sender as FrameworkElement);
        }
    }
}