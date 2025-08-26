using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_launcher
{
    public class MainDownloadTextBlock : UIManager
    {
        private string _mainDownloadTextBlockText = "Default value";

        public string Text
        {
            get { return _mainDownloadTextBlockText; }
            set
            {
                if (_mainDownloadTextBlockText != value)
                {
                    _mainDownloadTextBlockText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
