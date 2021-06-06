using System.Collections.Generic;

namespace Database.Models
{
    public class Topic : Entity
    {
        public virtual ICollection<Subtopic> Subtopic { get; set; }
    }
}
