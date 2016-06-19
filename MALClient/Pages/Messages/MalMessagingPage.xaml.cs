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
using MALClient.Models;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Messages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MalMessagingPage : Page
    {
        private MalMessagingViewModel ViewModel => DataContext as MalMessagingViewModel;

        public MalMessagingPage()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => ViewModel.Init();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var msg = e.AddedItems.First() as MalMessageModel;
            msg.IsRead = true;
        }
    }
}
