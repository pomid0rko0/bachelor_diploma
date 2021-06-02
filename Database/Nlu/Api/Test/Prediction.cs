namespace Database.Nlu.Api.Test
{
    public class Prediction
    {
        public string intent { get; set; }
        public string predicted { get; set; }
        public string text { get; set; }
        public double confidence { get; set; }
    }
}
