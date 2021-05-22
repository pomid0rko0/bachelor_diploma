using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }
        [Required]
        public string AnswerText { get; set; }

        [Required, ForeignKey("Intent")]
        public int IntentId { get; set; }
        public virtual Intent Intent { get; set; }

    }
}
