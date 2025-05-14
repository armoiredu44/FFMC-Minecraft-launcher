using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetBaseDir_Click(object sender, RoutedEventArgs e)
        {
            string[] keys = { "sha1", "url" };
            string jsonFile = File.ReadAllText(@"C:\Users\Ehssan\Documents\software\code\C#\C# guides\version_manifest_v2.json");
            JsonUtility jsonInstance = new JsonUtility(jsonFile);

            jsonInstance.GetPropertyPath("id", "1.20.1", out string? path);
            Debug.WriteLine(path);
            jsonInstance.GetProperties(keys, path, out List<object?> values);

            if (values.Count == 0)
                Debug.WriteLine("no properties found");
            foreach (object? value in values)
            {
                if (!(value == null))
                {
                    Debug.WriteLine(value.ToString());
                }
                else
                {
                    Debug.WriteLine("String is null");
                }
            }
        }

        private void btnDownloadMc_Click(object sender, RoutedEventArgs e)
        {
            LauncherUtility.DownloadMinecraft();
        }
    }
}