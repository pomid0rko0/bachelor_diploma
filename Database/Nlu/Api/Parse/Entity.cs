namespace Database.Nlu.Api.Parse
{
    public class NluEntity
    {
        public int start { get; set; }
        public int end { get; set; }
        public string value { get; set; }
        public string entity { get; set; }
        public double? confidence { get; set; }
    }
}
