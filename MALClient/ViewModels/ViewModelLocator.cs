﻿using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.ViewModels
{
    public class ViewModelLocator
    {
        private static bool _initialized;

        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (_initialized)
                return;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<HamburgerControlViewModel>();

            SimpleIoc.Default.Register<RecommendationsViewModel>();
            SimpleIoc.Default.Register<AnimeListViewModel>();
            SimpleIoc.Default.Register<AnimeDetailsPageViewModel>();
            SimpleIoc.Default.Register<SearchPageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
            SimpleIoc.Default.Register<ProfilePageViewModel>();
            SimpleIoc.Default.Register<HummingbirdProfilePageViewModel>();
            SimpleIoc.Default.Register<CalendarPageViewModel>();
            SimpleIoc.Default.Register<MalArticlesViewModel>();
            SimpleIoc.Default.Register<MalMessagingViewModel>();

            _initialized = true;
        }

        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static RecommendationsViewModel Recommendations
            => ServiceLocator.Current.GetInstance<RecommendationsViewModel>();

        public static HamburgerControlViewModel Hamburger
            => ServiceLocator.Current.GetInstance<HamburgerControlViewModel>();

        public static AnimeListViewModel AnimeList => ServiceLocator.Current.GetInstance<AnimeListViewModel>();

        public static AnimeDetailsPageViewModel AnimeDetails
            => ServiceLocator.Current.GetInstance<AnimeDetailsPageViewModel>();

        public static SearchPageViewModel SearchPage => ServiceLocator.Current.GetInstance<SearchPageViewModel>();
        public static SettingsPageViewModel SettingsPage => ServiceLocator.Current.GetInstance<SettingsPageViewModel>();

        public static ProfilePageViewModel ProfilePage => ServiceLocator.Current.GetInstance<ProfilePageViewModel>();

        public static HummingbirdProfilePageViewModel HumProfilePage
            => ServiceLocator.Current.GetInstance<HummingbirdProfilePageViewModel>();

        public static CalendarPageViewModel CalendarPage
            => ServiceLocator.Current.GetInstance<CalendarPageViewModel>();

        public static MalArticlesViewModel MalArticles
            => ServiceLocator.Current.GetInstance<MalArticlesViewModel>();

        public static MalMessagingViewModel MalMessaging
            => ServiceLocator.Current.GetInstance<MalMessagingViewModel>();


    }
}