using System.Windows;
using System.Text;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //oh boy this could extend for so long... truly a menace
            DataContext = new UIManager();

            Initialyser.InitialyseApp();
        }
        string? content;
        private async void btnGetBaseDir_Click(object sender, RoutedEventArgs e) //tests here ;)
        {
            if (content != null)
            {
                Debugger.SendInfo(content);
            }
            else
                Debugger.SendInfo("null");
        }

        private async void btnDownloadMc_Click(object sender, RoutedEventArgs e)
        {
            (bool success, content) = await MainDownloader.DownloadMinecraft("1.20.1"); //more tests

            if (!success)
            {
                Debugger.SendError(content);
                MessageBox.Show("big error");
            }

            if (String.IsNullOrEmpty(content))
            {
                Debugger.SendInfo("content is empty");
            }else
                Debugger.SendInfo("Finished");
        }
    }
}