using System.Collections.Generic;

using Database.Models.Entities;

namespace Database.Models
{
    public class Answer : EntityAnswer
    {
        public EntityAnswer RemoveReferences() => new EntityAnswer
        {
            Id = Id,
            Value = Value,
            FullAnswerUrl = FullAnswerUrl
        };
        public static EntityAnswer RemoveReferences(Answer a) => a.RemoveReferences();
        public virtual ICollection<Question> Question { get; set; }

    }
}
