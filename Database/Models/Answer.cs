using System.Collections.Generic;

namespace Database.Models
{
    public class Answer : EntityAnswer
    {
        public virtual ICollection<Question> Question { get; set; }

    }
}
