using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_launcher
{
    public class UIHelper : UIManager
    {
        private static bool isForceProgressBarsSmoothingActive = true;
        private static bool wasAnimationDone = true;
        private static double progressBarGoal = 0;

        private static void checkRules() //make it callable
        {
            Rules rules = new Rules();

            string? rulesText = rules.ScanRulesFile();
            if (rulesText == null)
            {
                return;
            }

            bool? forceProgressBarsSmoothing = Rules.GetRuleValueAsBool(rulesText, Rules.ConvertRulePathToCorrectformat("ui.progress_bars.force_progress_bars_smoothing.active"));
            switch (forceProgressBarsSmoothing)
            {
                case false:
                    isForceProgressBarsSmoothingActive = false;
                    break;

                case null:
                    isForceProgressBarsSmoothingActive = true; break;

                default:
                    break;
            }


        }

        public static async Task SetMainDownloadProgressBarMaximum(double maximum)
        {
            while (!wasAnimationDone)
            {
                await Task.Delay(1);
            }
            wasAnimationDone = false;
            Debugger.SendInfo($"new animation started, objective 0 (reset)");
            await MainDownloadProgressBar.SmoothlySetValue(0, 2f);
            MainDownloadProgressBar.Maximum = maximum;
            wasAnimationDone = true;
            Debugger.SendInfo("authorisation to start another download");
        }

        public static async void SmoothlySetMainDownloadProgressBarValue(double targetValue, bool isFinal = false, float duration = 5f) //BIG BRAIN FUNCTION - the bigger the duration, the smoother, but the less consistent
        {
            //Debugger.SendInfo($"Download target {targetValue}\nMaximum {UIManager.MainDownloadProgressBar.Maximum}\nValue {MainDownloadProgressBar.Value}");

            Task ignoreWarningCS4014;

            if (isFinal)
            {
                Debugger.SendInfo($"new animation started, objective {targetValue} (final)");
                wasAnimationDone = false;
                await MainDownloadProgressBar.SmoothlySetValue(targetValue, duration);
                progressBarGoal = 0;
                wasAnimationDone = true;
                return;
            }

            if (!isForceProgressBarsSmoothingActive)
            {
                ignoreWarningCS4014 = MainDownloadProgressBar.SmoothlySetValue(targetValue, duration);
                return;
            }

            if (wasAnimationDone)
            {
                Debugger.SendInfo($"new animation started, objective {targetValue} (smooth finised)");
                ignoreWarningCS4014 = MainDownloadProgressBar.SmoothlySetValue(targetValue, duration);
                progressBarGoal = targetValue;
                wasAnimationDone = false;
                return;
            }

            if (progressBarGoal != MainDownloadProgressBar.Value)
            {
                //Debugger.SendInfo($"{progressBarGoal} != {MainDownloadProgressBar.Value}");
                //Debugger.SendInfo("Blocked");
                return;
            }

            //Debugger.SendInfo($"{progressBarGoal} = {MainDownloadProgressBar.Value}");
            Debugger.SendInfo("Animation was done !");
            wasAnimationDone = true;
        }
    }
}
