public class AllTypes // a cool class, since it allows any types to be used in something that can contain a value, as long as the value is known in advance
{
    public string Type { get; set; }
    public object Value { get; set; }

    public AllTypes(string Type, object Value)
    {
        this.Type = Type;
        this.Value = Value;
    }
}
