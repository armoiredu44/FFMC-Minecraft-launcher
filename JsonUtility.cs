using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

public class JsonUtility
{
    private JsonDocument doc;
    private JsonElement root;
    private static readonly string pattern = @"[\[\]\.]";

    public JsonUtility(string jsonFile)
    {
        doc = JsonDocument.Parse(jsonFile);
        root = doc.RootElement;
    }

    #region "GetPropertyPath"
    public bool GetPropertyPath(string? key, object value, out string? foundPath)
    {
        return findPropertyPath(root, key, value, out foundPath);
    }

    private bool findPropertyPath(JsonElement element, string? key, object value, out string? foundPath, string? currentPath = null) //This method is a masterpiece, and why didn't I simply set the path as an array ? idk
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
                        foundPath = currentPath; //return the property's directory, instead of its full path. May be changed.
                        return true;
                    }

                    if (findPropertyPath(property.Value, key, value, out foundPath, newPath))
                        
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

                    if (key is null                                     //only find an array element if there's no name
                        && ObjectValueComparator.IsObjectEqualToElement(value, iteratedElement))
                    {
                        foundPath = currentPath;
                        return true;
                    }

                    index++;

                    if (findPropertyPath(iteratedElement, key, value, out foundPath, newPath))
                        
                    return true;

                    
                }
                break;
        }
        foundPath = null;
        return false;

    }



    #endregion

    #region GetProperties

    public bool GetProperties(string[] keys, string? path, out List<object?> values)
    {
        return findProperties(keys, path, out values);
    }

    private bool findProperties(string[] keys, string? path, out List<object?> values)
    {
        if (!String.IsNullOrEmpty(path))
        {
            string[] partsNullableOfPath = Regex.Split(path, pattern);
            string[] partsNotNullOfPath = partsNullableOfPath.Where(part => !String.IsNullOrEmpty(part)).ToArray();

            if (travelToPathAndFind(partsNotNullOfPath, keys, root, out values))
            {
                return true;
            }
        }
        else if (root.ValueKind == JsonValueKind.Array) // cases where path is null : root <- ; ↆ <-
        {
            values = [];
            return false;
        }
        else if (travelToPathAndFind([], keys, root, out values))
        {
            return true;
        }

        values = [];
        return false; // later
    }

    private bool travelToPathAndFind(string[] partsNotNullOfPath, string[] keys, JsonElement element, out List<object?> values) //returns true no matter what, maybe consider returning false if no keys matched
    {
        values = [];

        if (partsNotNullOfPath.Length == 0)
        {
            foreach (string key in keys) 
            {
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    if (!(property.Value.ValueKind == JsonValueKind.Array) && !(property.Value.ValueKind == JsonValueKind.Object))
                    {
                        if (property.Name == key)
                        {
                            values.Add(property.Value);
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

    public static string?[] Test(string text)
    {
        return [];
    }
}