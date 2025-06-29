using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

public class JsonUtility : Utilities //This class is so complicated, surely I grew many braincells from it | This is a mess. I'M REDOING THE WHOLE CLASS , and chatgpt is a damn genius
{
    private JsonDocument doc;
    private JsonElement root;

    public JsonUtility(string jsonFile)
    {
        doc = JsonDocument.Parse(jsonFile);
        root = doc.RootElement;
    }

    #region GetPropertyPath

    public bool GetPropertyPath(string? key, object value, out List<AllTypes> foundPath)
    {
        if (!findPropertyPath(root, key, value, [], out foundPath))
        {
            if (String.IsNullOrEmpty(key))
                Debugger.SendError($"couldn't find an element matching for the value :  \"{value}\"");
            else
                Debugger.SendError($"couldn't find a property matching for key :  \"{key} \" , and value  \"{value}\"");
            return false;
        }
        else
            return true;
    }

    private bool findPropertyPath(JsonElement element, string? key, object value, List<AllTypes> path, out List<AllTypes> modifiedPath) //finds a property's OR an element's path
    {
        //check the type, if array or object, loop inside, check for the key and/or the value and do all that again. If it's not, skip. 
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                int index = 0; //yes even though the default value when declaring an int should be 0, some uses of the non-explicitely-declared 0 aren't recognized
                path.Add(new AllTypes("string", "default"));
                int pathIndexArray = path.Count - 1; 
                foreach (JsonElement iteratedElement in element.EnumerateArray())
                {
                    if (pathIndexArray < path.Count)
                        path.RemoveRange(pathIndexArray, path.Count - pathIndexArray);
                    path.Add(new AllTypes("int", index));

                    if (String.IsNullOrEmpty(key) && ValueComparator.IsObjectEqualToElement(value, iteratedElement))
                    {
                        path.RemoveAt(path.Count - 1);
                        modifiedPath = path;
                        return true;
                    }

                    index++;

                    if (findPropertyPath(iteratedElement, key, value, path, out modifiedPath))
                        return true;
                }
                break;

            case JsonValueKind.Object:
                path.Add(new AllTypes("string", "default"));
                int pathIndexObject = path.Count - 1;
                foreach (JsonProperty iteratedProperty in element.EnumerateObject())
                {
                    if (pathIndexObject < path.Count)
                        path.RemoveRange(pathIndexObject, path.Count - pathIndexObject);
                    path.Add(new AllTypes("string", iteratedProperty.Name));

                    if (!String.IsNullOrEmpty(key) /* <-- this right here could be used at a lower nesting level to avoid unncessary compute time, but whatever it's so few */ && iteratedProperty.Name == key && ValueComparator.IsObjectEqualToElement(value, iteratedProperty.Value))
                    {
                        path.RemoveAt(path.Count - 1);
                        modifiedPath = path;
                        return true;
                    }

                    if (findPropertyPath(iteratedProperty.Value, key, value, path, out modifiedPath))
                        return true;
                }
                break;

            default:
                if (path.Count == 0)
                {
                    Debugger.SendError($"Json file is not valid, cannot search for key : \"{key}\", and value  \"{value}\"");
                    modifiedPath = [];
                }
                modifiedPath = path;
                return false;

        }
        modifiedPath = [];
        return false;
    }

    #endregion
    
    #region GetProperties
    
    public bool GetProperties(string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties)
    {
        return getPropertiesFromPath(root, keys, path, out foundProperties);
    }

    private bool getPropertiesFromPath(JsonElement element, string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties)
    {
        foreach (AllTypes part in path)
        {
            if (part.Type == "int")
            {
                try
                {
                    element = element[Convert.ToInt32(part.Value)];
                }
                catch (Exception e)
                {
                    Debugger.SendError($"couldn't convert oject value to Int32, due to error : {e}");
                    foundProperties = [];
                    return false;
                }
                continue;
            }
            else if (part.Type == "string")
            {
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    if (property.Name == part.Value.ToString())
                    {
                        element = property.Value;
                        continue;
                    }
                }
            }
            else
            {
                Debugger.SendError($"an element of the path doesn't have a valid type [{part.Type}]");
            }
        }

        if (getPropertiesOnceInPath(element, keys, out foundProperties))
            return true;
        else
        {
            Debugger.SendError("couldn't find properties in some context, but this line should not be reachable so no worries");
            foundProperties = [];
            return false;
        }
    }

    private bool getPropertiesOnceInPath(JsonElement element, string[] keys, out List<AllTypes> foundProperties) //consider making this return false in some cases.
    {
        foundProperties = [];
        bool hasFoundKey;
        foreach (string key in keys) { //key loop before so the results are stored in order
            hasFoundKey = false;
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (property.Name == key)
                {
                    foundProperties.Add(new AllTypes(ValueComparator.GetElementType(property.Value), property.Value));
                    hasFoundKey = true;
                    break;
                }
            }
            if (!hasFoundKey)
                foundProperties.Add(new AllTypes("", "")); //happens if the property isn't found
            
        } 
        return true;
    }
    
    
    /*
    public string Test(string key) {
        return subTest(root, key);
    }
    
    /*
    private string subTest(JsonElement root, string key) {
        foreach (JsonProperty property in root.EnumerateObject())
        {
            if (property.Name == key)
            {
                if (ValueComparator.GetElementType(property.Value) == "decimal")
                {
                    Debugger.SendInfo(property.Value.ToString());
                    return "DECIMAAAALL";
                } else if (ValueComparator.GetElementType(property.Value) == "int")
                {
                    Debugger.SendInfo(property.Value.ToString());
                    return "IIIIINNNT";
                }
               
                
            } 

        }
        return "didn't find it";
    }
    */    
    
    #endregion
    /* //NOW REDO THIS
    #region GetPathOfValueFromKey

    public bool GetPropertyPathOfValueFromKey(string key, out string? foundPath)
    {
        return findPropertyPathOfValueFromKey(root, key,out foundPath);
    }

    private bool findPropertyPathOfValueFromKey(JsonElement element, string key, out string? finalPath, string? currentPath = null) // It just works ! and easy to make
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Array: //won't find the path inside an array since a key is searched, it'll just go through the array
                int index = 0;
                foreach (JsonElement iteratedElement in element.EnumerateArray())
                {
                    string newPath;
                    if (currentPath == null)
                    {
                        newPath = $"[{index}]";
                    }
                    else
                    {
                        newPath = $"{currentPath}[{index}]";
                    }

                    index++;

                    if (findPropertyPathOfValueFromKey(iteratedElement, key, out finalPath, newPath))
                        return true; //may be the source of issues, remove indentation if so
                }
                break;

            case JsonValueKind.Object:
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    string newPath;
                    if (currentPath == null)
                    {
                        newPath = property.Name;
                    }
                    else
                    {
                        newPath = $"{currentPath}.{property.Name}";
                    }

                    if (property.Name == key)
                    {
                        finalPath = newPath; //newPath and not currentPath cuz we want the directory inside the property, not the directory the property is in.
                        return true;
                    }

                    if (findPropertyPathOfValueFromKey(property.Value, key, out finalPath, newPath))
                        return true; //may be the source of issues, remove indentation if so
                }
                break;
        }
        finalPath = null;
        return false;
    }

    #endregion

    #region loop over each properties and get a value from a key

    public bool GetValuesInEachPropertyofPath(string[] keys, string? path, out List<object?> values, out List<string?> types, int level = 1) //inside a path, you wanna iterate over each property/element and in each of them if you find the key you were searching for you get the value.
    { // also using a list cuz adding troubles and stuff. 
        IterateOverEachElementOfTheProperty(root, keys, path, level, out values, out types);
        return false;
    }
    
    private bool IterateOverEachElementOfTheProperty(JsonElement element, string[] keys, string? path, int level, out List<object?> values, out List<string?> types)
    {
        //First we gotta get to the property


    }

    

    

    private bool addValuesToOutputVariables(JsonProperty property, out List<object?> values, out List<string?> types)
    {
        types = [];
        foreach (JsonProperty iteratedProperty in property.Value.EnumerateObject())
        {
            object value = property.Value;

            switch (checkType(value.ToString()!))
            {
                case "int":
                    {
                        types.Add("number"); //This is horrible
                        break;
                    }
                case "string":
                    {
                        types.Add("string");
                        break;
                    }
                default:
                    {
                        break;
                    }
            }                    
        }
        
    }
    #endregion

    #region convert json element to int (assuming it is an int, gives a wrong result for floats)

    public static int? ConvertJsonElementToInt(object? value)
    {
        if (value is JsonElement element && element.ValueKind == JsonValueKind.Number)
        {
            return element.GetInt32();
        }
        MessageBox.Show("Given value cannot be converted to int");

        return null;
        
    }

    #endregion
    */
}