﻿using AppFrontend.ContentPages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MenuPage();
        }
    }
}
