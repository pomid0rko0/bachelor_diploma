using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Database.Models;

namespace Database.Data
{
    public class QAContext : IdentityDbContext
    {
        public QAContext(DbContextOptions<QAContext> options)
            : base(options)
        {
        }
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
