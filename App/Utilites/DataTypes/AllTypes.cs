public class AllTypes // DO BETTER this sucks
{
    public object Type { get; set; } //this should always be a string
    public object Value { get; set; }

    public AllTypes(object Type, object Value)
    {
        this.Type = Type;
        this.Value = Value;
    }
}
