using System.Windows;
using System.Windows.Input;

namespace Minecraft_launcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new UIManager();

            Initialyser.InitialyseApp();
        }
        string? content;
        private void btnGetBaseDir_Click(object sender, RoutedEventArgs e) //tests here ;)
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
            #region actualCode
            UIHelper.EnableDynamicMainDownloadProgressBarValue();
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
            await Task.Delay(5000);
            UIHelper.disableDynamicSmoothing();
            #endregion actualCode
            #region fakeDownload1
            /*
            await UIHelper.SetMainDownloadProgressBarMaximum(250000);

            for (int i = 0; i <= 250000; i += 250000 / 1000)
            {
                UIHelper.UpdateMainDownloadProgressBarTarget(i);
                await Task.Delay(100);
            }

            UIHelper.UpdateMainDownloadProgressBarTarget(250000, true);
            */
            #endregion fakeDownload1
            #region testToDiscernAnimation1
            /*
            //await UIHelper.SetMainDownloadProgressBarMaximum(100);
            Random random = new Random();

            UIHelper.UpdateMainDownloadProgressBarTarget(random.Next(0, 100), false, 1f);
            */
            #endregion testToDiscernAnimation1

            #region DynamicBarTesting
            /*
            UIElement? bar = Application.Current.MainWindow.FindName("mainDownloadProgressBar") as UIElement;
            var source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double transform = source.CompositionTarget.TransformToDevice.M11;

            

            double getTargetPos()
            {
                //You were trying to get the absolute mouse pos
                double windowLeft = Application.Current.MainWindow.Left * transform;
                double windowRight = windowLeft + (Application.Current.MainWindow.Width*transform);
                double MouseX = Mouse.GetPosition(bar).X;
                double MouseXToScreen = ;
                Debugger.SendInfo(windowLeft + " " + MouseXToScreen);
                if (MouseX < windowLeft)
                    return 0;
                if (MouseX > windowRight)
                    return 100;
                if (bar is not null)
                {
                    double MouseXProportion = MathExtra.Clamp01Double(MouseX / bar.RenderSize.Width);
                    return MouseXProportion * 100;
                }
                return 0;
                
            }

            
            UIHelper.EnableDynamicMainDownloadProgressBarValue(getTargetPos);
            */
            #endregion DynamicBarTesting
        }
    }
}