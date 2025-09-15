

namespace Minecraft_launcher
{
    public class MainDownloadProgressBar : UIManager
    {
        private double _mainDownloadProgressBarValue = 0; //default
        private double _mainDownloadProgressBarMaximum = 100;
        public double Value // do not change this or it will break
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

        public async void InitializeDynamicSmoothing(Func<double> getTargetPosition, Func<bool> stopDynamicSmoothing)
        {
            bool hasToStop = stopDynamicSmoothing();
            double velocity = 0.0d;
            float priorTime = (float)Stopwatch.Elapsed.TotalSeconds;
            MathExtra.Interpolations.Dynamics.SmoothDampFollower follower = new MathExtra.Interpolations.Dynamics.SmoothDampFollower();

            float deltaTime;
            float currentTime;
            float reactionTime = 0.3f;
            while (!hasToStop)
            {
                currentTime = (float)Stopwatch.Elapsed.TotalSeconds;
                deltaTime = currentTime - priorTime;
                priorTime = currentTime;
                Value = follower.SmoothDamp(Value, getTargetPosition(), ref velocity, reactionTime, deltaTime);
                //Debugger.SendInfo("Value is "+Value.ToString());
                await Task.Delay(16); //120h is more fluid even on 60Hz displays
                hasToStop = stopDynamicSmoothing();
            }
            Debugger.SendInfo("Loop ended");
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
