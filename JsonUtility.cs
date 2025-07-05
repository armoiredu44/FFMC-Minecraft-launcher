using System.Text.Json;

public class JsonUtility : Utilities // My braincell generation rate triples when I edit this class
{
    private JsonDocument doc;
    private JsonElement root;

    public JsonUtility(string jsonFile)
    {
        doc = JsonDocument.Parse(jsonFile);
        root = doc.RootElement;
    }

    #region GetPropertyPath

    public bool GetPropertyPath(string? key, object? value, out List<AllTypes> foundPath, bool isIncluded = false, bool useStructure = false)
    {
        if (useStructure)
        {
            return findPropertyPath_UseStructure(root, key, value, [], out foundPath, isIncluded);
        }
        else
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

    }

    public bool GetPropertyPath(JsonElement element, string? key, object? value, out List<AllTypes> foundPath, bool isIncluded = false, bool useStructure = false) //overload
    {
        if (useStructure)
        {
            return findPropertyPath_UseStructure(element, key, value, [], out foundPath, isIncluded);
        }
        else
        {
            if (!findPropertyPath(element, key, value, [], out foundPath, isIncluded))
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
    }
        

    private bool findPropertyPath(JsonElement element, string? key, object? value, List<AllTypes> path, out List<AllTypes> modifiedPath, bool isIncluded) //finds a property's OR an element's path
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
                int index = 0; //yes even though the default value when declaring an int should be 0, some uses of the non-explicitely-declared 0 aren't recognized
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

    private bool findPropertyPath_UseStructure(JsonElement element, string? key, object? value, List<AllTypes> path, out List<AllTypes> modifiedPath, bool isIncluded) //finds a property's OR an element's path
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
                int index = 0; //yes even though the default value when declaring an int should be 0, some uses of the non-explicitely-declared 0 aren't recognized
                path.Add(new AllTypes("int", ""));
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
                path.Add(new AllTypes("int", ""));
                int pathIndexObject = path.Count - 1;
                int indexObjet = 0;
                foreach (JsonProperty iteratedProperty in element.EnumerateObject())
                {
                    if (pathIndexObject < path.Count)
                        path.RemoveRange(pathIndexObject, path.Count - pathIndexObject);
                    path.Add(new AllTypes("int", indexObjet));

                    if (iteratedProperty.Name == key)
                    {
                        if (value is null)
                        {
                            if (!isIncluded)
                                path.RemoveAt(path.Count - 1);
                            modifiedPath = path;
                            return true;
                        }
                        else if (ValueComparator.IsObjectEqualToElement(value, iteratedProperty.Value))
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
                if (path.Count == 0) //The point of this ? 😂
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

    public bool GetProperties(string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties, bool useStructure = false)
    {
        if (useStructure) //big thing to do : independantely choose useStructure in getToPathFirst and GetPropetiesOnceInPath, BUT That is not useful for now
        {
            return getPropertiesFromPath_GetToPathFirstAndRelyOnStructure(root, keys, path, out foundProperties);
        }
        else
        {
            return getPropertiesFromPath_GetToPathFirst(root, keys, path, out foundProperties);
        }
    }

    public bool GetProperties(JsonElement element, string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties, bool useStructure = false) //overload for when a starting path that is not root is needed
    {
        if (useStructure)
        {
            return getPropertiesFromPath_GetToPathFirstAndRelyOnStructure(element, keys, path, out foundProperties);
        }
        else
        {
            return getPropertiesFromPath_GetToPathFirst(element, keys, path, out foundProperties);
        }
    }

    private bool getPropertiesFromPath_GetToPathFirst(JsonElement element, string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties)
    {
        foreach (AllTypes part in path)
        {
            if (part.Type == "int")//could've used a switch, maybe todo later
            {
                if (element.ValueKind != JsonValueKind.Array)
                {
                    foundProperties = [];
                    foreach (string key in keys)
                    {
                        foundProperties.Add(new AllTypes("", "")); //what happens if the properties cannot be reached -> same result if they cannot be found in their directory
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
            else if (part.Type == "string")
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

    private bool getPropertiesFromPath_GetToPathFirstAndRelyOnStructure(JsonElement element, string[] keys, List<AllTypes> path, out List<AllTypes> foundProperties)
    {
        foreach (AllTypes part in path)
        {
            if (part.Type == "int")//could've used a switch, maybe todo later
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:

                        int indexArray;
                        try
                        {
                            indexArray = Convert.ToInt32(part.Value);
                        }
                        catch (Exception e)
                        {
                            Debugger.SendError($" Mismatch type and value for a part of path. Details : {e}");
                            foundProperties = [];
                            return false;
                        }
                        if (indexArray > element.GetArrayLength() - 1) //we don't check if the index is negative, that shouldn't happen
                        {
                            foundProperties = [];
                            Debugger.SendError($" index out of range, cannot get to target path");
                            return false;
                        }

                        element = element[indexArray];
                        continue;

                    case JsonValueKind.Object:
                        int indexObject;
                        try
                        {
                            indexObject = Convert.ToInt32(part.Value);
                        }
                        catch (Exception e)
                        {
                            Debugger.SendError($" Mismatch type and value for a part of path. Details : {e}");
                            foundProperties = [];
                            return false;
                        }
                        if (indexObject > element.GetPropertyCount() - 1) //we don't check if the index is negative, that shouldn't happen
                        {
                            foundProperties = [];
                            Debugger.SendError($" index out of range, cannot get to target path");
                            return false;
                        }
                        try
                        {
                            element = element[indexObject];
                        }
                        catch (Exception e) //WARNING DON'T FORGET ABOUT THIS
                        {
                            Debugger.SendError($"can't access object's element via index :  {e}");
                        }
                        
                        break;

                    default:
                         foundProperties = [];
                        return false;
                       

                }


                continue;
            }
            else
            {
                Debugger.SendError($"an element of the path doesn't have a valid type for structure search : [{part.Type}]");
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

    #endregion

    #region GetElementPathfromPath

    public bool GetPropertyPath(List<AllTypes> from, string? key, object? value, out List<AllTypes> foundPath, bool isIncluded = false, bool useStructure = false) //overload
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

    private bool getElementFromPath(JsonElement element, List<AllTypes> path, out JsonElement foundElement)
    {
        foreach (AllTypes part in path)
        {
            if (part.Type == "int")
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
            else if (part.Type == "string")
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

    public bool GetValuesInElementList(List<AllTypes> elementPath /* <- where the iteration will take place */, string[] keys, List<List<AllTypes>> propertiesPaths, out List<List<AllTypes>> foundValues, bool isPatternInvariable = true)
    {
        return getPropertiesInElementList_GetToPathFirst(root, keys, elementPath, propertiesPaths, out foundValues);
    }
    
    private bool getPropertiesInElementList_GetToPathFirst(JsonElement element, string[] keys, List<AllTypes> elementPath, List<List<AllTypes>> propertiesPaths, out List<List<AllTypes>> foundValues)
    {
        foreach (AllTypes part in elementPath) //do this one then
        {
            if (part.Type == "int")
            {
                try
                {
                    element = element[Convert.ToInt32(part.Value)];
                }
                catch (Exception e)
                {
                    Debugger.SendError($"Path cannot be found, couldn't convert object value to Int, due to error : {e}");
                    foundValues = [];
                    return false;
                }
                continue;
            }
            else if (part.Type == "string")
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
    public static class PathEditor
    {
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