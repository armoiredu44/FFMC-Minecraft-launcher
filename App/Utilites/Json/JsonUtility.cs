using System.Text.Json;

public class JsonUtility : Utilities // My braincell generation rate triples when I edit this class
{
    private JsonDocument doc;
    private JsonElement root;

    /// <summary>
    /// Constructor of a JsonUtility instance.
    /// </summary>
    /// <param name="jsonFile">The json string that will be deserialized.</param>
    public JsonUtility(string jsonFile)
    {
        doc = JsonDocument.Parse(jsonFile);
        root = doc.RootElement;
    }

    #region GetPropertyPath

    /// <summary>
    /// Finds a property's path, starting from the root directory of the json document.
    /// </summary>
    /// <param name="key">If not null, will find the first property matching that key if it also matches with the value and can be found.</param>
    /// <param name="value">If not null, will find the first property matching that value if it also matches with the key and be found</param>
    /// <param name="foundPath">The path of the property if found.</param>
    /// <param name="isIncluded">If true, the out path will be the directory of the property itself. If false, the out path will be the directory containing the property</param>
    /// <returns>True if the property was found. False if the property wasn't found or if both the key and value were null.</returns>
    public bool GetPropertyPath(string? key, object? value, out List<AllTypes> foundPath, bool isIncluded = false) 
    {
        if (!findPropertyPath(root, key, value, [], out foundPath, isIncluded))
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

    /// <summary>
    /// Finds a property's path, starting from a given element inside the json document.
    /// </summary>
    /// <param name="element">The element the search starts from.</param>
    /// <param name="key">If not null, will find the first property matching that key if it also matches with the value and can be found.</param>
    /// <param name="value">If not null, will find the first property matching that value if it also matches with the key and be found</param>
    /// <param name="foundPath">The path of the property if found.</param>
    /// <param name="isIncluded">If true, the out path will be the directory of the property itself. If false, the out path will be the directory containing the property</param>
    /// <returns>True if the property was found. False if the property wasn't found or if both the key and value were null.</returns>
    public bool GetPropertyPath(JsonElement element, string? key, object? value, out List<AllTypes> foundPath, bool isIncluded = false) //overload
    {
        
        if (!findPropertyPath(element, key, value, [], out foundPath, isIncluded))
        {
            Debugger.SendError($"couldn't find a property matching for key :  \"{key} \" , and value  \"{value}\"");
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Private method that will find a property inside a json structure, recursive.
    /// </summary>
    /// <param name="element">The element the search starts from.</param>
    /// <param name="key">If not null, will find the first property matching that key if it also matches with the value and can be found.</param>
    /// <param name="value">If not null, will find the first property matching that value if it also matches with the key and be found</param>
    /// <param name="path">Used by itself when recursing, represents the current path the method is in</param>
    /// <param name="modifiedPath">If the property can be found, will out this list as the last path the method was in</param>
    /// <param name="isIncluded">If true, the out path will be the directory of the property itself. If false, the out path will be the directory containing the property</param>
    /// <returns>True if the property was found. False if the property wasn't found or if both the key and value were null.</returns>
    private static bool findPropertyPath(JsonElement element, string? key, object? value, List<AllTypes> path, out List<AllTypes> modifiedPath, bool isIncluded) //finds a property's OR an element's path
    {
        if (String.IsNullOrEmpty(key) && value == null)
        {
            Debugger.SendError("both key and value are false, cannot find the property");
            modifiedPath = [];
            return false;
        }
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                int index = 0;
                path.Add(new AllTypes("string", "default"));
                int pathIndexArray = path.Count - 1;
                foreach (JsonElement iteratedElement in element.EnumerateArray())
                {
                    if (pathIndexArray < path.Count)
                        path.RemoveRange(pathIndexArray, path.Count - pathIndexArray);
                    path.Add(new AllTypes("int", index));

                    if (value != null) //don't check for the value if it's null
                    {
                        if (String.IsNullOrEmpty(key) && ValueComparator.IsObjectEqualToElement(value, iteratedElement))
                        {
                            if (!isIncluded)
                                path.RemoveAt(path.Count - 1);
                            modifiedPath = path;
                            return true;
                        }
                    }

                    index++;

                    if (findPropertyPath(iteratedElement, key, value, path, out modifiedPath, isIncluded))
                        return true;
                }
                break;

            case JsonValueKind.Object: //apparently JsonElement.TryGetProperty does that for me, but whatever
                path.Add(new AllTypes("string", "default"));
                int pathIndexObject = path.Count - 1;
                foreach (JsonProperty iteratedProperty in element.EnumerateObject())
                {
                    if (pathIndexObject < path.Count)
                        path.RemoveRange(pathIndexObject, path.Count - pathIndexObject);
                    path.Add(new AllTypes("string", iteratedProperty.Name));

                    if (iteratedProperty.Name == key)
                    {
                        if (value is null)
                        {
                            if (!isIncluded)
                                path.RemoveAt(path.Count - 1);
                            modifiedPath = path;
                            return true;
                        } else if (ValueComparator.IsObjectEqualToElement(value, iteratedProperty.Value))
                        {
                            if (!isIncluded)
                                path.RemoveAt(path.Count - 1);
                            modifiedPath = path;
                            return true;
                        }
                    }

                    if (findPropertyPath(iteratedProperty.Value, key, value, path, out modifiedPath, isIncluded))
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

    /// <summary>
    /// Retreives properties' values from a property's path in a json structure.
    /// </summary>
    /// <param name="keys">The keys of the properties whose value will be retreived.</param>
    /// <param name="path">The path of the object that contains the properties whose value will be retreived.</param>
    /// <param name="foundProperties">A list of the values that were retreived, is in the same order as the keys[] array. Properties that weren't found will be empty.</param>
    /// <returns>True if the path was valid. False if the path didn't match with the json structure.</returns>
    public bool GetProperties(string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties) //add isIncluded for this PLEASE
    {
        return getPropertiesFromPath_GetToPathFirst(root, keys, path, out foundProperties);
    }

    /// <summary>
    /// Retreives properties' values from a property's path, starting at a given element of a json structure.
    /// </summary>
    /// <param name="element">The starting point of the search.</param>
    /// <param name="keys">The keys of the properties whose value will be retreived.</param>
    /// <param name="path">The path of the object that contains the properties whose value will be retreived.</param>
    /// <param name="foundProperties">A list of the values that were retreived, is in the same order as the keys[] array. Properties that weren't found will be empty.</param>
    /// <returns>True if the path was valid. False if the path didn't match with the json structure.</returns>
    public static bool GetProperties(JsonElement element, string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties) //overload for when a starting path that is not root is needed
    {
        
        return getPropertiesFromPath_GetToPathFirst(element, keys, path, out foundProperties);
    }

    /// <summary>
    /// Private method goes to the given path.
    /// </summary>
    /// <param name="element">Where the method starts from in a json structure.</param>
    /// <param name="keys">The keys of the properties whose value will be retreived.</param>
    /// <param name="path">The path of the object that contains the properties whose value will be retreived.</param>
    /// <param name="foundProperties">A list of the values that were retreived, is in the same order as the keys[] array. Properties that weren't found will be empty.</param>
    /// <returns>True if the path was valid. False if the path didn't match with the json structure.</returns>
    private static bool getPropertiesFromPath_GetToPathFirst(JsonElement element, string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties)
    {
        foreach (AllTypes part in path)
        {
            if (part.Type.ToString() == "int")//could've used a switch, maybe todo later
            {
                if (element.ValueKind != JsonValueKind.Array)
                {
                    foundProperties = [];
                    foreach (string key in keys)
                    {
                        foundProperties.Add(new AllTypes("", "")); //what happens if the properties cannot be reached -> same result as if they cannot be found inside their directory
                    }
                    return true;

                }
                int index;
                try
                {
                    index = Convert.ToInt32(part.Value);
                }
                catch (Exception e)
                {
                    Debugger.SendError($" Mismatch type and value for a part of path. Details : {e}");
                    foundProperties = [];
                    return false;
                }
                if (index > element.GetArrayLength() - 1) //we don't check if the index is negative, that shouldn't happen
                {
                    foundProperties = [];
                    foreach (string key in keys)
                    {
                        foundProperties.Add(new AllTypes("", ""));
                    }
                    return true;
                }

                element = element[index];
                continue;
            }
            else if (part.Type.ToString() == "string")
            {
                if (element.ValueKind != JsonValueKind.Object)
                {
                    foundProperties = [];
                    foreach (string key in keys)
                    {
                        foundProperties.Add(new AllTypes("", ""));
                    }
                    return true;

                }
                if (!element.TryGetProperty(part.Value.ToString()! /*<-part.Value cannot be null !*/, out element))
                {
                    foundProperties = [];
                    foreach (string key in keys)
                    {
                        foundProperties.Add(new AllTypes("", ""));
                    }
                    return true;
                }
            }
            else
            {
                Debugger.SendError($"an element of the path doesn't have a valid type [{part.Type}]");
                foundProperties = [];
                return false;
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

    /// <summary>
    /// Private method that retreives properties inside an json object.
    /// </summary>
    /// <param name="element">The json object the method will retreive peroperties from. MUST be an object or an exception will happen.</param>
    /// <param name="keys">The keys of the properties whose value will be retreived.</param>
    /// <param name="foundProperties">A list of the values that were retreived, is in the same order as the keys[] array. Properties that weren't found will be empty.</param>
    /// <returns>True no matter what lol.</returns>
    private static bool getPropertiesOnceInPath(JsonElement element, string[] keys, out List<AllTypes> foundProperties) //consider making this return false in some cases.
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

    #endregion

    #region GetElementPathfromPath

    /// <summary>
    /// Finds a property's path starting from a specific path of the json structure.
    /// </summary>
    /// <param name="from">Where the search starts from.</param>
    /// <param name="key">If not null, will find the first property matching that key if it also matches with the value and can be found.</param>
    /// <param name="value">If not null, will find the first property matching that value if it also matches with the key and be found.</param>
    /// <param name="foundPath">The path of the property if found.</param>
    /// <param name="isIncluded">If true, the out path will be the directory of the property itself. If false, the out path will be the directory containing the property.</param>
    /// <returns>True if the property if found. False if the given path wasn't accessible, or if the property couldn't be found.</returns>
    public bool GetPropertyPath(List<AllTypes> from, string? key, object? value, out List<AllTypes> foundPath, bool isIncluded = false) //overload
    {
        foundPath = [];

        if (!getElementFromPath(root, from, out JsonElement element)){
            Debugger.SendError("couldn't get to the given path");
            return false;
        }

        if (!findPropertyPath(element, key, value, [], out foundPath, isIncluded))
            {
                if (String.IsNullOrEmpty(key))
                    Debugger.SendError($"couldn't find an element matching for the value :  \"{value}\" , starting from given path");
                else
                    Debugger.SendError($"couldn't find a property matching for key :  \"{key} \" , and value  \"{value}\" , starting from given path");
                return false;
            }
            else
                return true;
    }

    /// <summary>
    /// Retreives the element at the given path of the json structure.
    /// </summary>
    /// <param name="element">Where the search starts from.</param>
    /// <param name="path">The path of the element to retreive.</param>
    /// <param name="foundElement">The element if found.</param>
    /// <returns>False if the path doesn't match with the json structure, if the path is wrong, or if the element isn't found (PLEASE VERIFY). True if the element is found.</returns>
    private bool getElementFromPath(JsonElement element, List<AllTypes> path, out JsonElement foundElement)
    {
        foreach (AllTypes part in path)
        {
            if (part.Type.ToString() == "int")
            {
                if (element.ValueKind != JsonValueKind.Array)
                {
                    foundElement = element;
                    return false;
                }
                int index;
                try
                {
                    index = Convert.ToInt32(part.Value);
                }
                catch (Exception e)
                {
                    Debugger.SendError($" Path unmatched, couldn't convert object value to Int32, due to error : {e}");
                    foundElement = element;
                    return false;
                }
                if (index > element.GetArrayLength() - 1) //we don't check if the index is negative, that shouldn't happen
                {
                    foundElement = element;
                    return false;
                }

                element = element[index];
                continue;
            }
            else if (part.Type.ToString() == "string")
            {
                if (element.ValueKind != JsonValueKind.Object)
                {
                    foundElement = element;
                    return false;
                }
                if (element.TryGetProperty(part.Value.ToString()!, out element))
                {
                    foundElement = element;
                    return true;
                }
            }
            else
            {
                Debugger.SendError($"an element of the path doesn't have a valid type [{part.Type}]");
                foundElement = element;
                return false;
            }
        }
        foundElement = element; //what happens when the element is found
        return true;

    }
    #endregion

    #region GetValuesInElementList

    /// <summary>
    /// 
    /// </summary>
    /// <param name="elementPath">The path of the element where the </param>
    /// <param name="keys"></param>
    /// <param name="propertiesPaths"></param>
    /// <param name="foundValues"></param>
    /// <param name="isPatternInvariable"></param>
    /// <returns></returns>
    public bool GetValuesInElementList(List<AllTypes> elementPath /* <- where the iteration will take place */, string[] keys, List<List<AllTypes>> propertiesPaths, out List<List<AllTypes>> foundValues, bool isPatternInvariable = true)
    {
        return getPropertiesInElementList_GetToPathFirst(root, keys, elementPath, propertiesPaths, out foundValues);
    }
    
    private bool getPropertiesInElementList_GetToPathFirst(JsonElement element, string[] keys, List<AllTypes> elementPath, List<List<AllTypes>> propertiesPaths, out List<List<AllTypes>> foundValues)
    {
        foreach (AllTypes part in elementPath) //do this one then
        {
            if (part.Type.ToString() == "int")
            {
                try
                {
                    element = element[Convert.ToInt32(part.Value)];
                }
                catch (Exception e)
                {
                    Debugger.SendError($"Path cannot be found, couldn't convert object value to Int due to error : {e}");
                    foundValues = [];
                    return false;
                }
                continue;
            }
            else if (part.Type.ToString() == "string")
            {
                if (!element.TryGetProperty(part.Value.ToString()!, out element))
                {
                    Debugger.SendInfo($"Path doesn't exist in the current context");
                    foundValues = [];
                    return false;
                }
            }
            else
            {
                Debugger.SendError($"an element of the path doesn't have a valid type [{part.Type}]");
                foundValues = [];
                return false;
            }
        }
        if (!getPropertiesInElementListOnceInPath(element, keys, propertiesPaths, out foundValues))
        {
            foundValues = [];
            return false;

        }
        return true;
    }

    private bool getPropertiesInElementListOnceInPath(JsonElement element, string[] keys, List<List<AllTypes>> propertiesPaths /* whut daa heeeeeelll*/, out List<List<AllTypes>> foundValues) //chaos is beautiful
    {
        foundValues = [];
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                foreach (JsonElement iteratedElement in element.EnumerateArray())
                {
                    foreach (List<AllTypes> path in propertiesPaths)
                    {
                        if (GetProperties(iteratedElement, keys, path, out List<AllTypes> foundProperties))
                        {
                            foundValues.Add(foundProperties);
                        }
                    }
                }
                return true; //make that useful
            case JsonValueKind.Object:
                //Debugger.SendInfo(element.ToString() + "\n PROPERTY  :");
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    //Debugger.SendInfo(property.ToString());
                    foreach (List<AllTypes> path in propertiesPaths)
                    {
                        if (GetProperties(property.Value, keys, path, out List<AllTypes> foundProperties))
                        {
                            foundValues.Add(foundProperties);
                        }
                    }
                }
                return true;
            default:
                Debugger.SendError("property is not the right type, cannot proceed");
                return false;
        }
    }
    #endregion

    #region PathEditor

    /// <summary>
    /// A class that edits paths of type 'AllTypes'.
    /// </summary>
    public static class PathEditor
    {
        /// <summary>
        /// Cuts a path.
        /// </summary>
        /// <param name="list">The list that will be cut.</param>
        /// <param name="before">Any element before that value will be cut.</param>
        /// <param name="result">The resulting path.</param>
        /// <returns>False if 'before' is lower than 0 or higher than the list count. True if the operation was successful.</returns>
        public static bool cutList(List<AllTypes> list, int before, out List<AllTypes> result)
        {
            if (before < 0 || before > list.Count)
            {
                Debugger.SendError($"couldn't cut list, index out of range, index : {before}, list size : {list.Count}");
                result = list;
                return false;
            }

            list.RemoveRange(0, before);
            result = list;
            return true;
        }
    }
    #endregion
}