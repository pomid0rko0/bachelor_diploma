using System.Collections.Generic;

namespace Database.Nlu
{
    public class Config
    {
        public class Rule
        {
            public string rule { get; set; }
            public virtual ICollection<object> steps { get; set; }

        }
        public class Intent
        {
            public string intent { get; set; }
            public virtual string examples { get; set; }
        }
        public class Response
        {
            public string text { get; set; }
        }
        public string language { get; set; }
        public virtual object pipeline { get; set; }

        public virtual object policies { get; set; }
        public virtual IEnumerable<Rule> rules { get; set; }
        public virtual IEnumerable<string> intents { get; set; }
        public virtual IEnumerable<Intent> nlu { get; set; }
        public virtual IDictionary<string, IEnumerable<Response>> responses { get; set; }

    }
}
