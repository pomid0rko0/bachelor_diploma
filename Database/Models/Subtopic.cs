using System.Collections.Generic;

using Database.Models.Entities;

namespace Database.Models
{
    public class Subtopic : EntitySubtopic
    {
        public EntitySubtopic WithoutReferences() => new EntitySubtopic
        {
            Id = Id,
            Value = Value,
            TopicId = TopicId
        };
        public static EntitySubtopic WithoutReferences(Subtopic st) => st.WithoutReferences();
        public virtual Topic Topic { get; set; }
        public virtual ICollection<Question> Question { get; set; }
    }
}
