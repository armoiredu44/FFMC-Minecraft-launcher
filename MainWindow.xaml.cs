﻿using System.IO;
using System.Windows;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Debugger.CreateLogFileAtStartup();
        }

        private void btnGetBaseDir_Click(object sender, RoutedEventArgs e) //tests here ;)
        {

            Debugger.SendInfo("pick directory button got clicked !");
        }

        private async void btnDownloadMc_Click(object sender, RoutedEventArgs e)
        {
            IMinecraftDownloadVersionManager? downloader = await RessourcesManager.DownloadMinecraft("1.20.1");
            bool hasDownloaded = await downloader.MainDownload();
            if (hasDownloaded)
                MessageBox.Show("success!");
        }
    }
}