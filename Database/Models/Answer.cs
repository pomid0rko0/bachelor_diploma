using System.Collections.Generic;

namespace Database.Models
{
    public class Answer : Entity
    {
        public virtual ICollection<Question> Question { get; set; }

    }
}
