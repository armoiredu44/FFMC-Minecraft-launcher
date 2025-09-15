using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Minecraft_launcher
{
    public class UIHelper : UIManager
    {
        private static bool isForceProgressBarsSmoothingActive = true;
        private static double targetPosition = 0;
        private static double maximumProportional = 0;
        private static bool stopDynamicSmoothing = false;

        private static void checkRules() //make it callable
        {
            Rules rules = new Rules();

            string? rulesText = rules.ScanRulesFile();
            if (rulesText == null)
            {
                return;
            }

            bool? forceProgressBarsSmoothing = Rules.GetRuleValueAsBool(rulesText, Rules.ConvertRulesPathToCorrectformat("ui.progress_bars.force_progress_bars_smoothing.active"));
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

        public static void SetMainDownloadProgressBarMaximum(double maximum) //progress bar always between 0 and 100
        {
            //Debugger.SendInfo("maximum set to " + maximum);
            targetPosition = 0; //also resets the bar to avoid it going over the maximum
            maximumProportional = maximum;
        }

        public static void UpdateMainDownloadProgressBarTarget(double targetValue)
        {
            //Debugger.SendInfo("targetValue updated " + (targetValue / maximumProportional) * 100);
            targetPosition = targetValue;
        }
        
        static bool isDynamicSmoothingDisabled()
        {
            return stopDynamicSmoothing;
        }

        public static void disableDynamicSmoothing()
        {
            stopDynamicSmoothing = true;
        }
        public static async void EnableDynamicMainDownloadProgressBarValue()
        {
            //Debugger.SendInfo("Initializing");
            stopDynamicSmoothing = false;
            UIManager.MainDownloadProgressBar.InitializeDynamicSmoothing(sendTargetPosition, isDynamicSmoothingDisabled);
        }

        private static double sendTargetPosition()
        {
            if (maximumProportional == 0)
                return 0;
            double positionPerCent = (targetPosition / maximumProportional) * 100;
            //Debugger.SendInfo("requested " + positionPerCent);
            return positionPerCent;
        }
    }
}
