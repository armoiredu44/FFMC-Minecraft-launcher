using System.Windows;
using System.IO;

using System.Diagnostics;

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

        private void btnDownloadMc_Click(object sender, RoutedEventArgs e)
        {
            IMinecraftDownloadVersionManager downloader =  RessourcesManager.DownloadMinecraft("1.20.1");
        }
    }
}