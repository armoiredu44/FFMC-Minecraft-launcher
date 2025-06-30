using System.Windows;
using System.IO;

using System.Diagnostics;
using System.Runtime.CompilerServices;

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

            JsonUtility jsonUtility = new JsonUtility(File.ReadAllText(@"C:\Users\Ehssan\Documents\code\C#\minecraft launcher\plans\1.20.1.json"));
            jsonUtility.GetPropertyPath("libraries", null, out List<AllTypes> mainPath, true);
            string[] keys = ["path", "sha1", "size", "url", "name"];
            jsonUtility.GetPropertyPath("artifact", null, out List<AllTypes> path_1, true);
            JsonUtility.PathEditor.cutList(path_1, mainPath.Count - 1, out List<AllTypes> path_1_Cut);
            jsonUtility.GetPropertyPath("name", "osx", out List<AllTypes> path_2);
            JsonUtility.PathEditor.cutList(path_2, mainPath.Count - 1, out List<AllTypes> path_2_Cut);
            List<List<AllTypes>> finalFormList = [path_1_Cut, path_2_Cut];

            jsonUtility.GetValuesInElementList(mainPath, keys, finalFormList, out List<List<AllTypes>> values);
            List<string> path = [];
            List<string> sha1 = []; //reminder, you were testing if that method above worked, now you need to extract the value from the output and display a part of them.
            List<AllTypes> size = [];
            List<AllTypes> url = [];
            List<AllTypes> name = [];

            foreach (List<AllTypes> list in values)
            {
                foreach (AllTypes value in list)
                {
                    switch (value.Type)
                    {
                        case "Int":
                            break;
                        case "String":
                            break;
                        default:
                            Debugger.SendError("type is not valid in this context");
                            break;
                    }
                }
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