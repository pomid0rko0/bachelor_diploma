using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public class Intent
    {
        [Key]
        public int IntentId { get; set; }
        [Required]
        public string IntentName { get; set; }

        [ForeignKey("Answer")]
        public int? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        
        public virtual ICollection<Question> Question { get; set; }

    }
}
