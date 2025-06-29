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

            Debugger.SendInfo("pick directory button got clicked !");

            JsonUtility jsonUtility = new JsonUtility(File.ReadAllText(@"C:\Users\Ehssan\Documents\code\C#\minecraft launcher\plans\1.20.1.json"));
            jsonUtility.GetPropertyPath("path", "io/netty/netty-transport-native-epoll/4.1.82.Final/netty-transport-native-epoll-4.1.82.Final-linux-x86_64.jar", out List<AllTypes> output);
            foreach (AllTypes element in output)
            {
                Debugger.SendInfo(element.Value.ToString()!);
            }
            string[] keys = ["sha1", "size", "url", "Hello"];
            jsonUtility.GetProperties(keys, output, out List<AllTypes> foundProperties);
            int index = 0;
            foreach (AllTypes property in foundProperties)
            {
                if (string.IsNullOrEmpty(property.Value.ToString()))
                {
                    Debugger.SendWarn($"property {keys[index]}'s value wasn't found");
                    continue;
                }
                Debugger.SendInfo(property.Value.ToString()!);
                index++;
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