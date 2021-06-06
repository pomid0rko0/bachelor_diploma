using System.ComponentModel.DataAnnotations;

namespace Database.Models.Entities
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
