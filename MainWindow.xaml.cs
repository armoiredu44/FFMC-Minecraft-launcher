using System.Windows;
using System.IO;

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window //The code-behind of the UI
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetBaseDir_Click(object sender, RoutedEventArgs e) //tests here ;)
        {
            JsonUtility utility = new JsonUtility(File.ReadAllText(@"C:\Users\Ehssan\Documents\software\code\C#\C# guides\1.20.1.json"));
            if (utility.GetPropertyPathOfValueFromKey("id", out string? path))
                Debug.WriteLine(path);
            else
            {
                Debug.WriteLine("Path not found");
            }
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