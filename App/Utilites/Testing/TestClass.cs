public class testClass
{
    //This code shows a practical use of provided methods to get values from a repetitive element, rthe fact that it works is beautiful
    /*          case 1
     *         
     *          JsonUtility jsonUtility = new JsonUtility(File.ReadAllText(@"C:\Users\Ehssan\Documents\code\C#\minecraft launcher\plans\1.20.1.json"));
                if (!jsonUtility.GetPropertyPath("libraries", null, out List<AllTypes> mainPath, true)) //We get the libraries path (path to what's in it)
                {
                    Debugger.SendError("could find libraries's path");
                }
                string[] keys = ["path", "sha1", "size", "url", "name"];
                string[] keys_1 = ["path", "sha1", "size", "url"];
                string[] keys_2 = ["name"];
                if (!jsonUtility.GetPropertyPath("artifact", null, out List<AllTypes> path_1, true)) //We get the artifact path( what's in it)
                {
                    Debugger.SendError("could find artifact's path");
                }

                JsonUtility.PathEditor.cutList(path_1, mainPath.Count + 1, out List<AllTypes> path_1_Cut); //We set the artifact path root as THE ELEMENT (setting it to libraties causes an issue)
                if (!jsonUtility.GetPropertyPath(mainPath, "name", "osx", out List<AllTypes> path_2)) //This overload starts from libraries, so it does not contain it and has 1 less part before the real path
                {
                    Debugger.SendInfo("couldn't get name : osx's path");
                }

                JsonUtility.PathEditor.cutList(path_2, mainPath.Count, out List<AllTypes> path_2_Cut); //we set the name path root as THE ELEMENT
                List<List<AllTypes>> finalFormList = [path_1_Cut, path_2_Cut];

                jsonUtility.GetValuesInElementList(mainPath, keys, finalFormList, out List<List<AllTypes>> values);

                Debugger.SendInfo($"there are {values.Count} elements");
                int a = 0;
                int i = 0;

                foreach (List<AllTypes> list in values)
                {
                    if (a == 0)
                    {
                        Debugger.SendInfo($"element n°{i + 1} : ");
                        int j = 0;
                        foreach (string key in keys_1)
                        {
                            Debugger.SendInfo($"{key} : {list[j].Value}");
                            j++;
                        }
                        a++;

                    }
                    else
                    {
                        int j = keys_1.Count();
                        foreach (string key in keys_2)
                        {
                            Debugger.SendInfo($"{key} : {list[j].Value}");
                            j++;
                        }
                        a = 0;
                        i++;
                    }
                }
            */
    /* case 2
     * 
*          JsonUtility jsonUtility = new JsonUtility(File.ReadAllText(@"C:\Users\Ehssan\Documents\code\C#\minecraft launcher\plans\5.json"));
        if (!jsonUtility.GetPropertyPath("objects", null, out List<AllTypes> objectsPath, true))
        {
            Debugger.SendError("could find objects");
        }
        string[] keys = ["hash", "size"];
        if (!jsonUtility.GetPropertyPath("icons/icon_128x128.png", null, out List<AllTypes> path_1, true))
        {
            Debugger.SendError("could find main property path");
        }
        JsonUtility.PathEditor.cutList(path_1, objectsPath.Count + 1, out List<AllTypes> path_1_Cut);
        List<List<AllTypes>> finalList = [path_1_Cut];
        jsonUtility.GetValuesInElementList(objectsPath, keys, finalList, out List<List<AllTypes>> foundValues);

        Debugger.SendInfo($"there are {foundValues.Count} elements");
        int i = 0;

        foreach (List<AllTypes> list in foundValues)
        {
            Debugger.SendInfo($"element n°{i + 1} : ");
            int j = 0;
            foreach (string key in keys)
            {
                Debugger.SendInfo($"{key} : {list[j].Value}");
                j++;
            }
            i++;
        }
    */
}
