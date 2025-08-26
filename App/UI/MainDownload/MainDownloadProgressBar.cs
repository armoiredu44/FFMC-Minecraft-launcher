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
        public double Value // do not change this or it will break
        {
            get { return _mainDownloadProgressBarValue; }
            set
            {
                if (_mainDownloadProgressBarValue != value)
                {
                    Debugger.SendInfo("ProgressBar value got set to : " + value.ToString());
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

        public async Task SmoothlySetValue(double targetValue, float duration)
        {
            Debugger.SendInfo("smooth start");
            int id = ++smoothingFunctionID;
            float elapsedTime = 0f;
            float time = (float)Stopwatch.Elapsed.TotalSeconds;

            while (Value != targetValue)
            {
                if (id != smoothingFunctionID)
                {
                    Debugger.SendInfo("smooth terminated");
                    return;
                }

                elapsedTime = (float)Stopwatch.Elapsed.TotalSeconds - time;
                float t = MathfExtra.Clamp01(elapsedTime / duration);
                Value = Value + (targetValue - Value) * t;
                double difference = Math.Abs(Value - targetValue);
                double precisionThreshold = (Maximum / 800);
                if (difference <= precisionThreshold) //arbitrary precision value
                {
                    Value = targetValue;
                    Debugger.SendInfo("smooth end");
                    return;
                } 
                await Task.Delay(3); //find a way to make it match the refresh rate, not important just yet
            }
        }

        public double Maximum // do not change this or it will break
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
