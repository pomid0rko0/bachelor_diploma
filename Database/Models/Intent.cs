using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class Intent
    {
        [Key]
        public int Id { set; get; }
        [Required]
        public string Name { set; get; }
        [Required]
        public ICollection<Question> Questions { set; get; }
        [Required]
        public ICollection<Answer> Answers { set; get; }
    }
}
