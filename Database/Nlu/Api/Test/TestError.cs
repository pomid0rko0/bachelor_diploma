using System.Collections.Generic;
using Database.Nlu.Api.Parse;

namespace Database.Nlu.Api.Test
{
    public abstract class TestError
    {
        public string text { get; set; }
    }
    public class TestEntity
    {
        public int start { get; set; }
        public int end { get; set; }
        public string value { get; set; }
        public string entity { get; set; }
        public double? confidence { get; set; }
    }
    public class TestIntent
    {
        public string name { get; set; }
        public double confidence { get; set; }
    }
    public class IntentTestError : TestError
    {
        public double intent_response_key_target { get; set; }
        public TestIntent intent_response_key_prediction { get; set; }
    }
    public class EntityTestError : TestError
    {
        public ICollection<TestEntity> entities { get; set; }
        public ICollection<TestEntity> predicted_entities { get; set; }
    }
    public class ResponseSelectorTestError : TestError
    {
        public string intent_response_key_target { get; set; }
        public TestIntent intent_response_key_prediction { get; set; }
    }
}
