using System.Collections.Generic;

using Database.Models.Entities;

namespace Database.Models
{
    public class Topic : EntityTopic
    {
        public EntityTopic WithoutReferences() => new EntityTopic
        {
            Id = Id,
            Value = Value,
        };
        public static EntityTopic WithoutReferences(Topic t) => t.WithoutReferences();
        public virtual ICollection<Subtopic> Subtopic { get; set; }
    }
}
