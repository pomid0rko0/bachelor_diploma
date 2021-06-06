using Database.Models.Entities;

namespace Database.Models
{
    public class Question : EntityQuestion
    {
        public EntityQuestion WithoutReferences() => new EntityQuestion
        {
            Id = Id,
            Value = Value,
            AnswerId = AnswerId,
            SubtopicId = SubtopicId,
            IsUiQuestion = IsUiQuestion
        };
        public static EntityQuestion WithoutReferences(Question q) => q.WithoutReferences();
        public virtual Subtopic Subtopic { get; set; }
        public virtual Answer Answer { get; set; }
    }
}
