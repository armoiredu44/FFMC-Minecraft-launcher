using System.IO;

namespace Minecraft_launcher
{
    public class Rules: Utilities //this class allows for communication between the app and the rules file
    {
        private static string localPath = AppDir;
        public static bool checkForRulesFile(string rulesPath = "")
        {
            if (string.IsNullOrEmpty(rulesPath))
                rulesPath = AppDir + @"/Settings/Rules/rules.json";

            if (!Path.Exists(rulesPath))
            {
                Debugger.SendInfo("rules.json couldn't be found, if this is the first startup you can ignore this message");
                if (!IoUtilities.Directory.CreateDirectory(rulesPath))
                    return false;
            }
            return false;
        }
    }
}
