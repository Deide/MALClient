﻿using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public sealed partial class AnimeDetailsPage : Page
    {
        public AnimeDetailsPage()
        {
            InitializeComponent();
        }

        private AnimeDetailsPageViewModel ViewModel => DataContext as AnimeDetailsPageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if (param == null)
                throw new Exception("No paramaters for this page");
            if (!param.AnimeMode)
                MainPivot.Items.RemoveAt(1);
            ViewModel.Init(param);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataContext = null;
            base.OnNavigatedFrom(e);
        }

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeWatchedCommand.Execute(null);
                WatchedEpsFlyout.Hide();
                e.Handled = true;
            }
        }

        private void SubmitReadVolumes(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeVolumesCommand.Execute(null);
                ReadVolumesFlyout.Hide();
                e.Handled = true;
            }
        }

        private void Pivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            if (!ViewModel._initialized)
                return;
            switch (args.Item.Tag as string)
            {
                case "Details":
                    ViewModel.LoadDetails();
                    break;
                case "Reviews":
                    ViewModel.LoadReviews();
                    break;
                case "Recomm":
                    ViewModel.LoadRecommendations();
                    break;
                case "Related":
                    ViewModel.LoadRelatedAnime();
                    break;
            }
        }


        //if there was no date different than today's chosen by the user , we have to manually trigger the setter as binding won't do it
        private void StartDatePickerFlyout_OnDatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            if (!ViewModel.StartDateValid)
                ViewModel.StartDateTimeOffset = ViewModel.StartDateTimeOffset;
        }

        private void EndDatePickerFlyout_OnDatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            if (!ViewModel.EndDateValid)
                ViewModel.EndDateTimeOffset = ViewModel.EndDateTimeOffset;
        }

        private void StartDate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ViewModel.StartDateValid)
                ResetStartDateFlyout.ShowAt(sender as FrameworkElement);
        }

        private void EndDate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ViewModel.EndDateValid)
                ResetEndDateFlyout.ShowAt(sender as FrameworkElement);
        }

        private void UIElement_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ImageSaveFlyout.ShowAt(sender as FrameworkElement);
        }


        private void ReadVolumesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ReadVolumesFlyout.Hide();
        }

        private void WatchedEpsButton_OnClick(object sender, RoutedEventArgs e)
        {
            WatchedEpsFlyout.Hide();
        }
    }
}