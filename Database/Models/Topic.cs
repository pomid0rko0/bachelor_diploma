using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class Topic
    {
        [Key]
        public int TopicId { get; set; }
        [Required]
        public string TopicText { get; set; }
        public virtual ICollection<Subtopic> Subtopic { get; set; }
    }
}
