using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
