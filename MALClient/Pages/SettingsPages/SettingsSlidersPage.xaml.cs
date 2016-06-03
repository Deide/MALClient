﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsSlidersPage : Page
    {
        private bool _initialized;

        public SettingsSlidersPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SliderReccommsToPull.Value = Settings.RecommsToPull;
            SliderReviewsToPull.Value = Settings.ReviewsToPull;
            SliderSeasonalToPull.Value = Settings.SeasonalToPull;
            _initialized = true;
        }

        private void ChangedReviewsToPull(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1)
                return;
            Settings.ReviewsToPull = (int)e.NewValue;
        }

        private void ChangedRecommsToPull(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1)
                return;
            Settings.RecommsToPull = (int)e.NewValue;
        }


        private void ChangedSeasonalToPull(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1)
                return;
            Settings.SeasonalToPull = (int)e.NewValue;
        }
    }
}
