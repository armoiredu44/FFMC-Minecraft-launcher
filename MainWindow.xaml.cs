using System.Windows;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window //The code-behind of the UI
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetBaseDir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDownloadMc_Click(object sender, RoutedEventArgs e)
        {
            IMinecraftDownloadVersionManager downloader =  RessourcesManager.DownloadMinecraft("1.20.1");
        }
    }
}