using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_launcher.App.UI.ProgressBars
{
    public class MainDownloadProgressBar : UIManager
    {
        private double _mainDownloadProgressBarValue = 0;
        private double _mainDownlaodProgressBarMaximum;
        public double MainDownloadProgressBarValue
        {
            get { return _mainDownloadProgressBarValue; }
            set
            {
                if (_mainDownloadProgressBarValue != value)
                {
                    _mainDownloadProgressBarValue = value;
                    OnPropertyChanged();
                }
                ;
            }
        }

        public double MainDownloadProgressBarMaximum
        {
            get { return _mainDownlaodProgressBarMaximum; }
            set 
            { 
                if (_mainDownlaodProgressBarMaximum != value) 
                {
                    _mainDownlaodProgressBarMaximum = value;
                    OnPropertyChanged();
                } 
            }
        }

    }
}
