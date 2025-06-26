using System.Text.Json;
public class testClass
{
    private JsonDocument doc;
    private JsonElement root;
    private static readonly string pattern = @"[\[\]\.]";
    
    private bool findProperty(JsonElement element, string[] keys, List<AllTypes> path, out List<AllTypes> values)
    {
        //check the type, if array or object, loop inside and do all that again. If it's not, skip. In any way check for the key if possible.
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                foreach (JsonElement iteratedElement in element.EnumerateArray())
                {
                    //continue here
                }
                break;

            case JsonValueKind.Object:
                break;

            default:

                break;
        }
    }
    
}
