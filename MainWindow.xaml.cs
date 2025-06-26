using System.Windows;
using System.IO;

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window//The code-behind of the UI
    {
        public MainWindow()
        {
            InitializeComponent();
            Debugger.CreateLogFileAtStartup();
        }

        private void btnGetBaseDir_Click(object sender, RoutedEventArgs e) //tests here ;)
        {
            MessageBox.Show(Environment.CurrentDirectory);
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