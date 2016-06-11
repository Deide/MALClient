using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using MALClient.UserControls;

namespace MALClient
{
    public static class HamburgerFlyoutService
    {
        private static HamburgerAnimeFiltersFlyout _animeFiltersFlyout;
        private static HamburgerMangaFiltersFlyout _mangaFiltersFlyout;
        private static HamburgerTopCategoriesFlyout _topCategoriesFlyout;

        private static HamburgerAnimeFiltersFlyout AnimeFiltersFlyout
            => _animeFiltersFlyout ?? (_animeFiltersFlyout = new HamburgerAnimeFiltersFlyout());

        private static HamburgerMangaFiltersFlyout MangaFiltersFlyout
            => _mangaFiltersFlyout ?? (_mangaFiltersFlyout = new HamburgerMangaFiltersFlyout());

        private static HamburgerTopCategoriesFlyout TopCategoriesFlyout
            => _topCategoriesFlyout ?? (_topCategoriesFlyout = new HamburgerTopCategoriesFlyout());


        public static void ShowAnimeFiltersFlyout(FrameworkElement placement)
        {
            AnimeFiltersFlyout.ShowAt(placement);
        }

        public static void ShowAnimeMangaFiltersFlyout(FrameworkElement placement)
        {
            MangaFiltersFlyout.ShowAt(placement);
        }

        public static void ShowTopCategoriesFlyout(FrameworkElement placement)
        {
            TopCategoriesFlyout.ShowAt(placement);
        }
    }
}
