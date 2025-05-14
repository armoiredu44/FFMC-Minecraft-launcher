using System.Text.Json;

public static class ObjectValueComparator
{
    public static bool IsObjectEqualToElement(object value, JsonElement element)
    {
        switch (value)
        {
            case string s: //how could I possibly discover this... If the object value is a string, it put it's value to s, typed string.
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
}
