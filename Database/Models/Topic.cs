using System.Collections.Generic;

using Database.Models.Entities;

namespace Database.Models
{
    public class Topic : EntityTopic
    {
        public EntityTopic RemoveReferences() => new EntityTopic
        {
            Id = Id,
            Value = Value,
        };
        public static EntityTopic RemoveReferences(Topic t) => t.RemoveReferences();
        public virtual ICollection<Subtopic> Subtopic { get; set; }
    }
}
