using System.Collections.Generic;

namespace Database.Nlu.Api.Test
{
    public class TestResult
    {
        public Evalution intent_evalution { get; set; }
        public Evalution response_selection_evaluation { get; set; }
        public Dictionary<string, Evalution> entity_evaluation { get; set; }
    }
}
