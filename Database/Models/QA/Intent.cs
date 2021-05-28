using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models.QA
{
    public class Intent : Entity
    {

        [ForeignKey("Answer")]
        public int? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        
        public virtual ICollection<Question> Question { get; set; }

    }
}
