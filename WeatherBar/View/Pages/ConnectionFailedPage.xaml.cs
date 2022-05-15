﻿using System.Windows.Controls;
using WeatherBar.Core;
using WeatherBar.ViewModel;

namespace WeatherBar.View.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy ConnectionFailedPage.xaml
    /// </summary>
    public partial class ConnectionFailedPage : Page
    {
        public ConnectionFailedPage()
        {
            InitializeComponent();
            ViewModelManager.CreateViewModel<ConnectionFailedPanelViewModel>(this);
        }
    }
}
