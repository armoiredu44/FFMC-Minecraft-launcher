using System.Text.Json;
public class testClass
{
    private JsonDocument doc;
    private JsonElement root;
    
    private bool findProperty(JsonElement element, string? key, object value, List<AllTypes> path, out List<AllTypes> modifiedPath) //finds a property's OR an element's path
    {
        //check the type, if array or object, loop inside, check for the key and/or the value and do all that again. If it's not, skip. 
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                int index = 0; //yes even though the default value when declaring an int should be 0, some uses of the non-explicitely-declared 0 aren't reconglized
                foreach (JsonElement iteratedElement in element.EnumerateArray())
                {
                    path.Add(new AllTypes("int", index));

                    if (String.IsNullOrEmpty(key) && ObjectValueComparator.IsObjectEqualToElement(value, iteratedElement)){ //kinda useless extra work but whatever
                        modifiedPath = path;
                        return true;
                    }
                        
                    index++;

                    if (findProperty(iteratedElement, key, value, path, out modifiedPath))
                        return true;
                }
                break;

            case JsonValueKind.Object:
                foreach (JsonProperty iteratedProperty in element.EnumerateObject())
                {
                    path.Add(new AllTypes("string", iteratedProperty.Name));

                    if (!String.IsNullOrEmpty(key) && iteratedProperty.Name == key && ObjectValueComparator.IsObjectEqualToElement(value, iteratedProperty.Value)){
                        modifiedPath = path;
                        return true;
                    }

                    if (findProperty(iteratedProperty.Value, key, value, path, out modifiedPath))
                        return true;
                }
                break;

            default:
                Debugger.SendError($"Json file is not valid, cannot search for key : \"{key} \", and value  \"{value} \"");
                modifiedPath = [];
                return false;
                
        }

        if (String.IsNullOrEmpty(key))
            Debugger.SendError($"couldn't find an element matching for the value :  \"{value} \"");
        else
            Debugger.SendError($"couldn't find a property matching for key :  \"{key} \" , and value  \"{value} \"");
        modifiedPath = [];
        return false;
    }
    
}
