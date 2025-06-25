using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

public class testClass
{
    private JsonDocument doc;
    private JsonElement root;
    private static readonly string pattern = @"[\[\]\.]";
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
}
