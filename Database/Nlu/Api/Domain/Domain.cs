using System.Collections.Generic;

namespace Database.Nlu.Api.Domain
{
    public class Domain
    {
        public class DomainIntent
        {
            public bool? use_entities { get; set; }
        }
        public Config config { get; set; }
        public ICollection<Dictionary<string, DomainIntent>> intents { get; set; }
        public ICollection<string> entities { get; set; }
        public Dictionary<string, Response> responses { get; set; }
        public ICollection<string> actions { get; set; }
    }
}