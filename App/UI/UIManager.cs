using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minecraft_launcher
{
    public class UIManager : Utilities, INotifyPropertyChanged //maaan, I gotta do it by sections that's annoying
    {
        public static UIManager _instance; //Only one instance for the whole project, so that it's always the same as the one used by the UI
        public static UIManager Instance => _instance ??= new UIManager();

        public event PropertyChangedEventHandler? PropertyChanged; 

        public void OnPropertyChanged([CallerMemberName] string propertyName = null) //this updates the UI
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        


        




    }
}
