using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.QA
{
    public class Subtopic
    {
        [Key]
        public int SubtopicId { get; set; }
        [Required]
        public string SubtopicText { get; set; }
        [Required, ForeignKey("Topic")]
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual ICollection<Question> Question { get; set; }
    }
}
