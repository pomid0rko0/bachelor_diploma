using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.Entities
{
    public class EntitySubtopic : Entity
    {
        [Required, ForeignKey("Topic")]
        public int TopicId { get; set; }
    }
}
