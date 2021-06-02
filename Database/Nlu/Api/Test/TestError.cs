using System.Collections.Generic;
using Database.Nlu.Api.Parse;

namespace Database.Nlu.Api.Test
{
    public abstract class TestError 
    {
        public string text { get; set; }
    }
    public class IntentTestError : TestError
    {
        public double intent_response_key_target { get; set; }
        public Parse.ParseResult.ResultIntent intent_response_key_prediction { get; set; }
    }
    public class EntityTestError : TestError
    {
        public ICollection<NluEntity> entities { get; set; }
        public ICollection<NluEntity> predicted_entities { get; set; }
    }
    public class ResponseSelectorTestError : TestError
    {
        public string intent_response_key_target { get; set; }
        public Parse.ParseResult.ResultIntent intent_response_key_prediction { get; set; }
    }
}
