namespace Demo.Base
{
    public class MultiString
    {
        public string? Chinese { get; set; }
        public string? English { get; set; }
        public SubMultiString Second { get; set; }
    }
    public class SubMultiString
    {
        public string? Name { get; set; }
    }
}
