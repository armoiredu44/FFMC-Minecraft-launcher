using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

public class JsonUtility : Utilities //This class is so complicated, surely I grew many braincells from it
{
    private JsonDocument doc;
    private JsonElement root;
    private static readonly string pattern = @"[\[\]\.]"; // This is stupid, why use a string as the path if you can use an array ?

    public JsonUtility(string jsonFile)
    {
        doc = JsonDocument.Parse(jsonFile);
        root = doc.RootElement;
    }

    #region GetPropertyPath

    public bool GetPropertyPath(string? key, object value, out string? foundPath)
    {
        return findPropertyPath(root, key, value, out foundPath);
    } 

    private bool findPropertyPath(JsonElement element, string? key, object value, out string? finalPath, string? currentPath = null) //This method is a masterpiece, and why didn't I simply set the path as an array ? idk
    {

        switch (element.ValueKind)
        {
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

                    if (property.Name == key
                        && ObjectValueComparator.IsObjectEqualToElement(value, property.Value))
                    {
                        finalPath = currentPath; //return the property's directory, instead of its full path. May be changed. //tf did I write ???
                        return true;
                    }

                    if (findPropertyPath(property.Value, key, value, out finalPath, newPath))
                        
                        return true;
                }
                break;

            case JsonValueKind.Array:
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

                    if (key is null                                     //only finds an array element if there's no name
                        && ObjectValueComparator.IsObjectEqualToElement(value, iteratedElement))
                    {
                        finalPath = currentPath; //WHy not nexPath ? Oh yeah cuz newPath is inside the property, while we want the directory the property is in.
                        return true;
                    }

                    index++;

                    if (findPropertyPath(iteratedElement, key, value, out finalPath, newPath))
                        return true; //may be the source of issues, remove indentation if so

                    
                }
                break;
        }
        finalPath = null;
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
        if (!String.IsNullOrEmpty(path))
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

    private string checkType(string part)
    {
        if (int.TryParse(part, out _)) //I'll complete it later, to move to the type class
        {
            return "int";
        }
        else if (bool.TryParse(part, out _))
        {
            return "bool";
        }
        else
        {
            return "string";
        }
    }

    #endregion

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

    public bool GetKeyInEachPropertiesInPath(string[] keys, string? path, out string?[] values)
    {
        values = [];
        return false;
    }

    #endregion

    #region convert json element to int (assuming it is an int, this is INSECURE)

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

}