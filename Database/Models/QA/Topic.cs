using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Models.QA
{
    public class Topic : Entity
    {
        public virtual ICollection<Subtopic> Subtopic { get; set; }
    }
}
