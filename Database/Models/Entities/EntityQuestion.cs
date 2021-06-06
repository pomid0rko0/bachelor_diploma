using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Entities
{
    public class EntityQuestion : Entity
    {
        public bool IsUiQuestion { get; set; }
        [Required, ForeignKey("Subtopic")]
        public int SubtopicId { get; set; }
        [Required, ForeignKey("Answer")]
        public int AnswerId { get; set; }
    }
}
