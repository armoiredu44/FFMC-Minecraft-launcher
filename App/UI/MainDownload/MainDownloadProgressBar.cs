using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Minecraft_launcher
{
    public class MainDownloadProgressBar : UIManager
    {
        private double _mainDownloadProgressBarValue = 0; //default
        private double _mainDownloadProgressBarMaximum = 100;
        private int numberOfSmoothFunctionUsed = 0;
        public double MainDownloadProgressBarValue
        {
            get { return _mainDownloadProgressBarValue; }
            set
            {
                if (_mainDownloadProgressBarValue != value)
                {
                    //Debugger.SendInfo("ProgressBar value got set to : " + value.ToString());
                    _mainDownloadProgressBarValue = value;
                    /*
                    if (_mainDownloadProgressBarMaximum == _mainDownloadProgressBarValue)
                    {
                        Debugger.SendInfo("maximum reached !");
                    }*/
                    OnPropertyChanged();
                }
                ;
            }
        }

        public void SmoothlySetMainDownloadProgressBarValue(double value)
        {
            numberOfSmoothFunctionUsed += 1;
            int numberOfSmoothFunctionUsedLocal = numberOfSmoothFunctionUsed;
            double difference = value - _mainDownloadProgressBarValue;
            if (difference == 0)
            {
                numberOfSmoothFunctionUsed -= 1;
                return;
            }
            else if (difference < 0)
            {
                while (difference < 0)
                {
                    MainDownloadProgressBarValue -= 1;
                    if (numberOfSmoothFunctionUsed > numberOfSmoothFunctionUsedLocal)
                    {
                        numberOfSmoothFunctionUsed -= 1;
                            return;
                    }
                }
                numberOfSmoothFunctionUsed -= 1;
                return;
            }
            else
            {
                while (difference > 0)
                {
                    MainDownloadProgressBarValue += 1;
                    if (numberOfSmoothFunctionUsed > numberOfSmoothFunctionUsedLocal)
                    {
                        numberOfSmoothFunctionUsed -= 1;
                        return;
                    }
                }
                numberOfSmoothFunctionUsed -= 1;
                return;
            }
        }

        public double MainDownloadProgressBarMaximum
        {
            get { return _mainDownloadProgressBarMaximum; }
            set 
            { 
                if (_mainDownloadProgressBarMaximum != value) 
                {
                    //Debugger.SendInfo("ProgressBar Maximum got set to : " + value.ToString());
                    _mainDownloadProgressBarMaximum = value;
                    OnPropertyChanged();
                } 
            }
        }
        
    }
}
