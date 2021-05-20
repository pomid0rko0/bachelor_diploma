using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Data
{
    public class QAContext : DbContext
    {
        public QAContext(DbContextOptions<QAContext> options)
            : base(options)
        {
        }
        public DbSet<Intent> Intents { set; get; }
        public DbSet<Answer> Answers { set; get; }
        public DbSet<Question> Questions { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
