using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class Answer
    {
        [Key]
        public int Id { set; get; }
        [Required]
        public string Text { set; get; }
        [Required]
        public Intent Intent { set; get; }

    }
}