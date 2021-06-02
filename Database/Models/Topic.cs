using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class Topic : Entity
    {
        public virtual ICollection<Subtopic> Subtopic { get; set; }
    }
}
