using System.Text.Json;

public class ValueComparator : Utilities
{
    public static bool IsObjectEqualToElement(object value, JsonElement element)
    {
        switch (value)
        {
            case string s:
                {
                    return element.ValueKind == JsonValueKind.String && element.ToString() == s; // That sh is brilliant, too bad I didn't find it out myself.
                }
            case int i:
                {
                    return element.ValueKind == JsonValueKind.Number && element.GetInt32() == i;
                }
            case bool b:
                {
                    return element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False && element.GetBoolean() == b;
                }
            case double d:
                {
                    return element.ValueKind == JsonValueKind.Number && element.GetDouble() == d;
                }
            default:
                return false;
        }
    }

    public static string GetElementType(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return "string";
            case JsonValueKind.Number:
                if (element.TryGetInt32(out int _)) //doesn't work with TryGetDecimal
                    return "int";
                else
                    return "decimal";
            case JsonValueKind.True:
                return "bool";
            case JsonValueKind.False:
                return "bool";
            case JsonValueKind.Null:
                return "null";
            case JsonValueKind.Undefined:
                return "undefined";
            case JsonValueKind.Object:
                return "object";
            case JsonValueKind.Array:
                return "array";
            default:
                return "Error";
        }
    }
    
}
