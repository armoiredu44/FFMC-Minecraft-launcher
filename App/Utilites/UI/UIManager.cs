using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minecraft_launcher
{
    public class UIManager : Utilities, INotifyPropertyChanged
    {
        public static UIManager _instance; //Only one instance for the whole project, so that it's always the same as the one used by the UI
        public static UIManager Instance => _instance ??= new UIManager();
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _mainDownloadTextBlock = "default value";

        public void OnPropertyChanged([CallerMemberName] string propertyName = null) //this updates the UI
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string MainDownloadTextBlock
        {
            get { return _mainDownloadTextBlock; }
            set
            {
                if ( _mainDownloadTextBlock != value)
                {
                    _mainDownloadTextBlock = value;
                    OnPropertyChanged();                    
                }
            }
        }

        

    }
}
