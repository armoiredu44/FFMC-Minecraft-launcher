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

            JsonUtility jsonUtility = new JsonUtility(File.ReadAllText(@"C:\Users\Ehssan\Documents\software\code\C#\C# guides\1.20.1.json"));
            if (!jsonUtility.GetPropertyPath("libraries", null, out List<AllTypes> mainPath, true)) //We get the libraries path (path to what's in it)
            {
                Debugger.SendError("could find libraries's path");
            }
            string[] keys = ["path", "sha1", "size", "url", "name"];
            string[] keys_1 = ["path", "sha1", "size", "url"];
            string[] keys_2 = ["name"];
            if (!jsonUtility.GetPropertyPath("artifact", null, out List<AllTypes> path_1, true)) //We get the artifact path( what's in it)
            {
                Debugger.SendError("could find artifact's path");
            }
            
            JsonUtility.PathEditor.cutList(path_1, mainPath.Count + 1, out List<AllTypes> path_1_Cut); //We set the artifact path root as THE ELEMENT (setting it to libraties causes an issue)
            if (!jsonUtility.GetPropertyPath(mainPath, "name", "osx", out List<AllTypes> path_2)) //This overload starts from libraries, so it does not contain it and has 1 less part before the real path
            {
                Debugger.SendInfo("couldn't get name : osx's path");
            }
            
            JsonUtility.PathEditor.cutList(path_2, mainPath.Count, out List<AllTypes> path_2_Cut); //we set the name path root as THE ELEMENT
            List<List<AllTypes>> finalFormList = [path_1_Cut, path_2_Cut];

            jsonUtility.GetValuesInElementList(mainPath, keys, finalFormList, out List<List<AllTypes>> values);
            
            Debugger.SendInfo($"there are {values.Count} elements");
            int a = 0;
            for (int i = 0; i == values.Count - 1; i++)
            {
                Debugger.SendInfo($"element n°{i + 1} : ");
                a = 0;
                foreach (List<AllTypes> list in values)
                {
                    foreach (AllTypes value in list)
                    {

                    }
                    if (a == 0)
                    {
                        int j = 0;
                        foreach (string key in keys_1)
                        {
                            Debugger.SendInfo($"{key} : {list[j].Value}");
                            j++;
                        }
                        a++;
                    }
                    else
                    {
                        int j = 0;
                        foreach (string key in keys_2)
                        {
                            Debugger.SendInfo($"{key} : {list[j].Value}");
                            j++;
                        }
                        a = 0;
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