namespace Database.Nlu.Api.Information
{
    public class Status
    {
        public object fingerprint { get; set; }
        public string model_file { get; set; }
        public int? num_active_training_jobs { get; set; }
    }
}