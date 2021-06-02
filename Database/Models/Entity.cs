using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
