﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Pages;
using MALClient.UserControls;

namespace MALClient.ViewModels
{
    public interface IHamburgerControlView
    {
        double GetScrollBurgerActualHeight();
    }

    public class Parameter<T> : INotifyPropertyChanged //wrapper
    {
        private T _value;
        public T Value //real value
        {
            get { return _value; }
            set { _value = value; RaisePropertyChanged("Value"); }
        }

        public Parameter(T value)
        {
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class HamburgerControlViewModel : ViewModelBase
    {
        private bool? _prevState;
        private int _stackPanelHeightSum = Creditentials.Authenticated ? 325 : 375; //base value , we are either on log in page or list page (app bar on/off)
        private bool _subtractedHeightForButton = true;


        public IHamburgerControlView View { get; set; }


        public Dictionary<string, Parameter<Brush>> TxtForegroundBrushes { get; } = new Dictionary<string, Parameter<Brush>>
        {
            ["AnimeList"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
            ["AnimeSearch"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
            ["LogIn"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
            ["Settings"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
            ["Profile"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
            ["Seasonal"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
            ["About"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
            ["Recommendations"] = new Parameter<Brush>(new SolidColorBrush(Colors.Black)),
        };

        public RelayCommand PaneOpenedCommand { get; private set; }

        private double _gridSeparatorHeight;
        public double GridSeparatorHeight
        {
            get { return _gridSeparatorHeight; }
            set
            {
                _gridSeparatorHeight = value;
                RaisePropertyChanged(() => GridSeparatorHeight);
            }
        }

        private double _gridBtmMarginHeight;
        public double GridBtmMarginHeight
        {
            get { return _gridBtmMarginHeight; }
            set
            {
                _gridBtmMarginHeight = value;
                RaisePropertyChanged(() => GridBtmMarginHeight);
            }
        }

        private BitmapImage _userImage;
        public BitmapImage UserImage
        {
            get { return _userImage; } 
            set
            {
                _userImage = value;
                RaisePropertyChanged(() => UserImage);
            }
        }

        private bool _profileButtonVisibility;
        public bool ProfileButtonVisibility
        {
            get { return _profileButtonVisibility; }
            set
            {
                _profileButtonVisibility = value;
                RaisePropertyChanged(() => ProfileButtonVisibility);
            }
        }

        private ICommand _buttonNavigationCommand;
        public ICommand ButtonNavigationCommand
        {
            get
            {
                return _buttonNavigationCommand ?? (_buttonNavigationCommand = new RelayCommand<Object>(ButtonClick));
            }
        }

        private async void ButtonClick(object o)
        {
            if(o == null)
                return;
            PageIndex page;
            if (PageIndex.TryParse(o as string, out page))
            {
                await
                    Utils.GetMainPageInstance()
                        .Navigate(page, page == PageIndex.PageSeasonal ? new AnimeListPageNavigationArgs() : null);
                SetActiveButton(Utils.GetButtonForPage(page));
            }
        }

        public HamburgerControlViewModel()
        {
            PaneOpenedCommand = new RelayCommand(this.PaneOpened);
        }

        public void ChangeBottomStackPanelMargin(bool up)
        {
            if (up == _prevState)
                return;

            _prevState = up;

            _stackPanelHeightSum += up ? 50 : -50;
        }    

        public void PaneOpened()
        {
            var val = Convert.ToInt32(View.GetScrollBurgerActualHeight());
            GridSeparatorHeight = val - _stackPanelHeightSum < 0 ? 0 : val - _stackPanelHeightSum;
            GridBtmMarginHeight = GridSeparatorHeight < 1 ? 50 : 0;
        }

        internal async Task UpdateProfileImg(bool dl = true)
        {
            if (Creditentials.Authenticated)
            {
                try
                {
                    StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("UserImg.png");
                    BasicProperties props = await file.GetBasicPropertiesAsync();
                    if (props.Size == 0)
                        throw new FileNotFoundException();
                    var bitmap = new BitmapImage();
                    using (var fs = (await file.OpenStreamForReadAsync()).AsRandomAccessStream())
                    {
                        bitmap.SetSource(fs);
                    }
                    UserImage = bitmap;
                }
                catch (FileNotFoundException)
                {
                    if (dl)
                        Utils.DownloadProfileImg();
                }
                catch (Exception)
                {
                    // ignored
                }

                ProfileButtonVisibility = true;
                if (_subtractedHeightForButton)
                {
                    _stackPanelHeightSum += 35;
                    _subtractedHeightForButton = false;
                }
            }
            else
            {
                ProfileButtonVisibility = false;
                if (!_subtractedHeightForButton)
                {
                    _stackPanelHeightSum -= 35;
                    _subtractedHeightForButton = true;
                }
            }


        }

        private void ResetActiveButton()
        {
            foreach (var foregroundBrush in TxtForegroundBrushes)
            {
                foregroundBrush.Value.Value = new SolidColorBrush(Colors.Black);
            }
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            ResetActiveButton();
            TxtForegroundBrushes[val.ToString()].Value = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
        }
    }
}