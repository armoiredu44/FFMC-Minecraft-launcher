using System;

public class AllTypes
{
    string Type { get; set; }
    object Value { get; set; }

    public AllTypes(string Type, object Value)
    {
        this.Type = Type;
        this.Value = Value;
    }
}
