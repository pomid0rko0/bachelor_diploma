using System.Collections.Generic;

namespace Database.Nlu.Api.Test
{
    public class Evalution
    {
        public IDictionary<string, IDictionary<string, double>> report { get; set; }
        public double accuracy { get; set; }
        public double f1_score { get; set; }
        public double precision { get; set; }
        ICollection<Prediction> predictions { get; set; }
        ICollection<TestError> errors { get; set; }
    }
}
