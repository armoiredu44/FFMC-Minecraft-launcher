using System.Windows;
using System.Windows.Media;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = UIManager.Instance;
            Debugger.CreateLogFileAtStartup();
                
        }

        private async void btnGetBaseDir_Click(object sender, RoutedEventArgs e) //tests here ;)
        {
            while (true)
            {
                Width += 1;
                await Task.Delay(2);
            }
            Debugger.SendInfo("pick directory button got clicked !");
        }

        private async void btnDownloadMc_Click(object sender, RoutedEventArgs e)
        {
            (bool success, string? content) = await RessourcesManager.DownloadMinecraft("1.20.1");
            if (String.IsNullOrEmpty(content))
            {
                Debugger.SendInfo("content is empty");
            }else
                Debugger.SendInfo(content);
            
            
        }
    }
}