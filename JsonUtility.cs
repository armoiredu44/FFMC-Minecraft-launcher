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

                    if (String.IsNullOrEmpty(key) && ObjectValueComparator.IsObjectEqualToElement(value, iteratedElement))
                    { //kinda useless extra work but whatever
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

                    if (!String.IsNullOrEmpty(key) && iteratedProperty.Name == key && ObjectValueComparator.IsObjectEqualToElement(value, iteratedProperty.Value))
                    {
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

    public bool GetProperties(string[] keys, string? path, out List<object?> values, out List<string?> types) //TODO : actually make the type thing work, DO IT NOW | I forgor how this works
    {
        return findProperties(keys, path, out values, out types);
    }

    private bool findProperties(string[] keys, string? path, out List<object?> values, out List<string?> types)
    {
        if (!String.IsNullOrEmpty(path)) //This separate each port of the path into an element of an array, so we just have to loop over the array
        {
            string[] partsNullableOfPath = Regex.Split(path, pattern);
            string[] partsNotNullOfPath = partsNullableOfPath.Where(part => !String.IsNullOrEmpty(part)).ToArray();

            if (travelToPathAndFind(partsNotNullOfPath, keys, root, out values, out types))
            {
                return true;
            }
        }
        else /* cases where path is null : root <- ; ↆ <- */if (root.ValueKind == JsonValueKind.Array) 
        {
            values = [];
            types = [];
            return false;
        }
        else if (travelToPathAndFind([], keys, root, out values, out types))
        {
            return true;
        }

        values = [];
        types = [];
        return false; // later
    }

    private bool travelToPathAndFind(string[] partsNotNullOfPath, string[] keys, JsonElement element, out List<object?> values, out List<string?> types) //returns true no matter what, maybe consider returning false if no keys matched, and make the lists as arrays since the keys' lenghts is defined prior.
    {
        values = [];
        types = [];

        if (partsNotNullOfPath.Length == 0)
        {
            foreach (string key in keys) 
            {
                foreach (JsonProperty property in root.EnumerateObject()) // So you can have multiples objects at root level (what if the root level is an array ? NOT SECURE)
                {
                    if (!(property.Value.ValueKind == JsonValueKind.Array) && !(property.Value.ValueKind == JsonValueKind.Object))
                    {
                        if (property.Name == key)
                        {
                            values.Add(property.Value);
                            types.Add(property.Value.ValueKind.ToString());
                        }
                    }
                }
            }

            return true;
        }
        else
        {
            foreach (string part in partsNotNullOfPath) // This sets element at the path
            {
                string type = checkType(part);
                switch (type)
                {
                    case "int":
                        {
                            element = element[int.Parse(part)]; //to do :  make it safe, what if it goes beyond the array's lenght ?
                            break;
                        }
                    case "string":
                        {
                            if (!element.TryGetProperty(part, out _))
                            {
                                MessageBox.Show($"Erreur : propriété {part} non trouvée dans (attention peut être long) : {element}");
                                break;
                            }
                            else
                            {
                                element = element.GetProperty(part);
                                break;
                            }
                        }
                    default:
                        {
                            MessageBox.Show($"Erreur : propriété {part} non trouvée dans (attention peut être long) : {element}");
                            return false;
                        }
                }

            }

            foreach (string key in keys) //this makes it add the values in the same order as the key
            {
                foreach (JsonProperty iteratedProperty in element.EnumerateObject()) // The final directory, an object, and the actual thing that has to happen
                {
                    if (!(iteratedProperty.Value.ValueKind == JsonValueKind.Array) && !(iteratedProperty.Value.ValueKind == JsonValueKind.Object))
                    {
                        if (iteratedProperty.Name == key)
                        {
                            values.Add(iteratedProperty.Value);
                            types.Add(iteratedProperty.Value.ValueKind.ToString());
                        }
                    }
                }
            }

            return true;
        }
    }

    private string checkType(string? part) // I named that "part" in the path part context, but it's general
    {
        if (int.TryParse(part, out _)) //I'll complete it later, to move to the type class (later : if it exists)
        {
            return "int";
        }
        else if (bool.TryParse(part, out _))
        {
            return "bool";
        }
        else if (part == null)
        {
            return "null";
        }
        else
        {
            return "string";
        }
    }

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