using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Minecraft_launcher
{
    public class MainDownloadProgressBar : UIManager
    {
        private double _mainDownloadProgressBarValue = 0; //default
        private double _mainDownloadProgressBarMaximum = 100;
        private int smoothingFunctionID = 0;
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

        public async Task SmoothlySetMainDownloadProgressBarValue(double targetValue, float duration = 2f)
        {
            int id = ++smoothingFunctionID;
            float elapsedTime = 0f;
            float time = (float)Stopwatch.Elapsed.TotalSeconds;

            while (MainDownloadProgressBarValue != targetValue)
            {
                if (id != smoothingFunctionID) return;

                elapsedTime = (float)Stopwatch.Elapsed.TotalSeconds - time;
                float t = MathfExtra.Clamp01(elapsedTime / duration);
                MainDownloadProgressBarValue = MainDownloadProgressBarValue + (targetValue - MainDownloadProgressBarValue) * t;
                double difference = Math.Abs(MainDownloadProgressBarValue - targetValue);
                double precisionThreshold = (MainDownloadProgressBarMaximum / 800);
                if (difference <= precisionThreshold) //arbitrary precision value
                {
                    MainDownloadProgressBarValue = targetValue;
                    return;
                } 
                await Task.Delay(3); //find a way to make it match the refresh rate
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
