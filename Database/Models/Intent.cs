using System.Collections.Generic;

namespace Database.Models
{
    public class Intent
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public virtual ICollection<Question> Questions { set; get; }
        public virtual ICollection<Answer> Answers { set; get; }

#nullable enable
        public bool IsSame(int? otherId = null, string? otherName = null)
        {
            var OId = otherId ?? Id; 
            var OName = otherName ?? Name;
            return Id == otherId && Name == otherName;
        }
    }
}