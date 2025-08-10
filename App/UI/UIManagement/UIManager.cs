using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minecraft_launcher
{
    public class UIManager : Utilities, INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler? PropertyChanged; 

        public void OnPropertyChanged([CallerMemberName] string propertyName = null) //this updates the UI
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //This is basically magic, but could go on for so long...
        //Register here every control that needs to be accessed from the back-end
        public static MainDownloadProgressBar MainDownloadProgressBar { get; } = new MainDownloadProgressBar();
        public static MainDownloadTextBlock MainDownloadTextBlock { get; } = new MainDownloadTextBlock();








    }
}
