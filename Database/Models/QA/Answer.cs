using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.QA
{
    public class Answer : Entity
    {

        [Required, ForeignKey("Intent")]
        public int IntentId { get; set; }
        public virtual Intent Intent { get; set; }

    }
}
