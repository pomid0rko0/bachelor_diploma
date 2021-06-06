using Database.Models.Entities;

namespace Database.Models
{
    public class Question : EntityQuestion
    {
        public EntityQuestion RemoveReferences() => new EntityQuestion
        {
            Id = Id,
            Value = Value,
            AnswerId = AnswerId,
            SubtopicId = SubtopicId,
            IsUiQuestion = IsUiQuestion
        };
        public static EntityQuestion RemoveReferences(Question q) => q.RemoveReferences();
        public virtual Answer Answer { get; set; }
    }
}
