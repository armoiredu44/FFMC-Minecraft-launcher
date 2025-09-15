using System.IO;

namespace Minecraft_launcher
{
    public class Rules: Utilities //this class allows for communication between the app and the rules file
    {
        private static string localPath = AppDir;
        private static string rulesPath = "";
        public static bool CheckForRulesFile(string rulesPath = "")
        {
            if (string.IsNullOrEmpty(rulesPath))
                rulesPath = localPath + @"/Settings/Rules/rules.json";

            if (!Path.Exists(rulesPath))
            {
                Debugger.SendInfo("rules.json couldn't be found, if this is the first startup you can ignore this message");
                if (!IoUtilities.Directory.CreateDirectory(rulesPath))
                    return false;
            }
            return true;
        }

        public string? ScanRulesFile()
        {
            if (!IoUtilities.File.ReadAllText(rulesPath, out string rulesText))
                return null;
            return rulesText;
        }

        public static List<AllTypes> ConvertRulesPathToCorrectformat(string rulePath)
        {
            string[] arrayToConvertToRightFormat = rulePath.Split('.');

            List<AllTypes> result = new List<AllTypes>();

            foreach (string item in arrayToConvertToRightFormat)
            {
                result.Add(new AllTypes("string", item));
            }

            return result;


        }

        public static string? GetRuleValueAsString(string? rulesText,  List<AllTypes> rulePath)
        {
            if (String.IsNullOrEmpty(rulesText))
                return null;
            JsonUtility serializer = new JsonUtility(rulesText);

            int lastValueOfRulePathIndex = rulePath.Count - 1;

            string rule = rulePath[lastValueOfRulePathIndex].Value.ToString()!;

            rulePath.RemoveAt(lastValueOfRulePathIndex);

            serializer.GetProperties([rule], rulePath, out List<AllTypes> foundSingleProperty);

            return foundSingleProperty[0].Value.ToString();
        }

        public static bool? GetRuleValueAsBool(string? rulesText, List<AllTypes> rulePath)
        {
            if (String.IsNullOrEmpty(rulesText))
                return null;
            JsonUtility serializer = new JsonUtility(rulesText);

            int lastValueOfRulePathIndex = rulePath.Count - 1;

            string rule = rulePath[lastValueOfRulePathIndex].Value.ToString()!;

            rulePath.RemoveAt(lastValueOfRulePathIndex);

            serializer.GetProperties([rule], rulePath, out List<AllTypes> foundSingleProperty);

            try
            {
                return (bool)foundSingleProperty[0].Value;
            }
            catch (Exception ex) {
                Debugger.SendError("Couldn't convert rule value from object to boolean");
                return null;
            }
        }

        public static int? GetRuleValueAsInt(string? rulesText, List<AllTypes> rulePath)
        {
            if (String.IsNullOrEmpty(rulesText))
                return null;
            JsonUtility serializer = new JsonUtility(rulesText);

            int lastValueOfRulePathIndex = rulePath.Count - 1;

            string rule = rulePath[lastValueOfRulePathIndex].Value.ToString()!;

            rulePath.RemoveAt(lastValueOfRulePathIndex);

            serializer.GetProperties([rule], rulePath, out List<AllTypes> foundSingleProperty);

            try
            {
                return (int)foundSingleProperty[0].Value;
            }
            catch (Exception ex)
            {
                Debugger.SendError("Couldn't convert rule value from object to integer");
                return null;
            }
        }

        public static double? GetRuleValueAsDouble(string? rulesText, List<AllTypes> rulePath)
        {
            if (String.IsNullOrEmpty(rulesText))
                return null;
            JsonUtility serializer = new JsonUtility(rulesText);

            int lastValueOfRulePathIndex = rulePath.Count - 1;

            string rule = rulePath[lastValueOfRulePathIndex].Value.ToString()!;

            rulePath.RemoveAt(lastValueOfRulePathIndex);

            serializer.GetProperties([rule], rulePath, out List<AllTypes> foundSingleProperty);

            try
            {
                return (double)foundSingleProperty[0].Value;
            }
            catch (Exception ex)
            {
                Debugger.SendError("Couldn't convert rule value from object to double");
                return null;
            }
        }

        public static float? GetRuleValueAsfloat(string? rulesText, List<AllTypes> rulePath)
        {
            if (String.IsNullOrEmpty(rulesText))
                return null;
            JsonUtility serializer = new JsonUtility(rulesText);

            int lastValueOfRulePathIndex = rulePath.Count - 1;

            string rule = rulePath[lastValueOfRulePathIndex].Value.ToString()!;

            rulePath.RemoveAt(lastValueOfRulePathIndex);

            serializer.GetProperties([rule], rulePath, out List<AllTypes> foundSingleProperty);

            try
            {
                return (float)foundSingleProperty[0].Value;
            }
            catch (Exception ex)
            {
                Debugger.SendError("Couldn't convert rule value from object to float");
                return null;
            }
        }

        public static long? GetRuleValueAsLong(string? rulesText, List<AllTypes> rulePath)
        {
            if (String.IsNullOrEmpty(rulesText))
                return null;
            JsonUtility serializer = new JsonUtility(rulesText);

            int lastValueOfRulePathIndex = rulePath.Count - 1;

            string rule = rulePath[lastValueOfRulePathIndex].Value.ToString()!;

            rulePath.RemoveAt(lastValueOfRulePathIndex);

            serializer.GetProperties([rule], rulePath, out List<AllTypes> foundSingleProperty);

            try
            {
                return (long)foundSingleProperty[0].Value;
            }
            catch (Exception ex)
            {
                Debugger.SendError("Couldn't convert rule value from object to long");
                return null;
            }
        }
    }
}
