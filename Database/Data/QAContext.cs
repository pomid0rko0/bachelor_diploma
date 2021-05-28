using Microsoft.EntityFrameworkCore;
using Database.Models.QA;

namespace Database.Data
{
    public class QAContext : DbContext
    {
        public QAContext(DbContextOptions<QAContext> options)
            : base(options)
        {
        }
        public DbSet<Intent> Intents { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Subtopic> Subtopics { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
