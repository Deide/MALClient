﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Flyouts
{
    public sealed partial class WatchedEpisodesFlyout : FlyoutPresenter
    {

        public WatchedEpisodesFlyout()
        {
            this.InitializeComponent();
        }

        public void ShowAt(FrameworkElement target)
        {
            DataContext = target.DataContext;
            WatchedEpsFlyout.ShowAt(target);
        }

        private void TxtBoxWatchedEps_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
         
            (DataContext as AnimeItemViewModel).OnFlyoutEpsKeyDown.Execute(e);
            WatchedEpsFlyout.Hide();
        }

        private void WatchedEpsFlyout_OnClosed(object sender, object e)
        {
            DataContext = null;
        }

        private void BtnSubmitOnClick(object sender, RoutedEventArgs e)
        {
            WatchedEpsFlyout.Hide();
        }
    }
}
