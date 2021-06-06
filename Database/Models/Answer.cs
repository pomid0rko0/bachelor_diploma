using System.Collections.Generic;

using Database.Models.Entities;

namespace Database.Models
{
    public class Answer : EntityAnswer
    {
        public EntityAnswer WithoutReferences() => new EntityAnswer
        {
            Id = Id,
            Value = Value,
            FullAnswerUrl = FullAnswerUrl
        };
        public static EntityAnswer WithoutReferences(Answer a) => a.WithoutReferences();
        public virtual ICollection<Question> Question { get; set; }

    }
}
