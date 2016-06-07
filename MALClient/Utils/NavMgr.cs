using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using MALClient.Pages;
using MALClient.ViewModels;
using System;

namespace MALClient
{
    /// <summary>
    ///     Back navigation manager , highly stripped down (compared to mobile version)
    ///     as on desktop we have separeate windows section dedicated to details and such...
    /// </summary>
    public static class NavMgr
    {
        #region BackNavigation
        private static ICommand _currentOverride;
        private static ICommand _currentOverrideMain;
        private static readonly Stack<AnimeDetailsPageNavigationArgs> _detailsNavStack =
            new Stack<AnimeDetailsPageNavigationArgs>(10);
        private static readonly Stack<ProfilePageNavigationArgs> _profileNavigationStack =
            new Stack<ProfilePageNavigationArgs>(10);

        public static void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
        {
            _detailsNavStack.Push(args);
            ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Visible;
        }

        public static void RegisterBackNav(ProfilePageNavigationArgs args)
        {
            _profileNavigationStack.Push(args);
            ViewModelLocator.Main.NavigateMainBackButtonVisibility = Visibility.Visible;
        }

        public static void CurrentViewOnBackRequested()
        {
            if (_currentOverride != null)
            {
                _currentOverride.Execute(null);
                _currentOverride = null;
                if (_detailsNavStack.Count == 0)
                    ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Collapsed;
                return;
            }

            if (_detailsNavStack.Count == 0) //when we are called from mouse back button
                return;

            ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails, _detailsNavStack.Pop());
            if (_detailsNavStack.Count == 0)
                ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Collapsed;
        }

        public static void ResetBackNav()
        {
            _detailsNavStack.Clear();
            _currentOverride = null;
            ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Collapsed;
        }

        public static void RegisterOneTimeOverride(ICommand command)
        {
            _currentOverride = command;
            ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Visible;
        }

        public static void RegisterOneTimeMainOverride(ICommand command)
        {
            _currentOverrideMain = command;
            ViewModelLocator.Main.NavigateMainBackButtonVisibility = Visibility.Visible;
        }

        internal static void CurrentMainViewOnBackRequested()
        {
            if (_currentOverrideMain != null)
            {
                _currentOverrideMain.Execute(null);
                _currentOverrideMain = null;
                ViewModelLocator.Main.NavigateMainBackButtonVisibility = Visibility.Collapsed;
            }


            if (_profileNavigationStack.Count == 0) //when we are called from mouse back button
                return;

            ViewModelLocator.Main.Navigate(PageIndex.PageProfile, _profileNavigationStack.Pop());
            if (_detailsNavStack.Count == 0)
                ViewModelLocator.Main.NavigateMainBackButtonVisibility = Visibility.Collapsed;
        }

        #endregion
    }
}