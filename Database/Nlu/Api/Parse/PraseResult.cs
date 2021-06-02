using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Database.Nlu.Api.Parse
{
    public class ParseResult
    {
        public class ResultIntent
        {
            public double id { get; set; }
            public string name { get; set; }
            public double confidence { get; set; }
        }
        public class ResponseSelector
        {
            public class Default
            {
                public class ResponseResult
                {
                    public class Text
                    {
                        public string text { get; set; }
                    }
                    public double id { get; set; }
                    public ICollection<Text> responses { get; set; }
                    public ICollection<Text> response_templates { get; set; }
                    public double confidence { get; set; }
                    public string intent_response_key { get; set; }
                    public string utter_action { get; set; }
                    public string template_name { get; set; }
                }
                public class Rank
                {
                    public double id { get; set; }
                    public double confidence { get; set; }
                    public string intent_response_key { get; set; }
                }
                public ResponseResult response { get; set; }
                public ICollection<Rank> ranking { get; set; }
            }
            public ICollection<string> all_retrieval_intents { get; set; }
            [JsonPropertyName("default")]
            public Default _default { get; set; }
        }
        public string text { get; set; }
        public ResultIntent intent { get; set; }
        public ICollection<NluEntity> entities { get; set; }
        public ICollection<ResultIntent> intent_ranking { get; set; }
        public ResponseSelector response_selector { get; set; }
    }
}
