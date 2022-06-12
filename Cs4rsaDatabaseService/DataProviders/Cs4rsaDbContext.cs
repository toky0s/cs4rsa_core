using Cs4rsaDatabaseService.Models;

using Microsoft.EntityFrameworkCore;

namespace Cs4rsaDatabaseService.DataProviders
{
    public class Cs4rsaDbContext : DbContext
    {
        public Cs4rsaDbContext() : base()
        {

        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionDetail> SessionDetails { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ProgramSubject> ProgramSubjects { get; set; }
        public DbSet<PreParSubject> PreParSubjects { get; set; }
        public DbSet<ParProDetail> ParProDetails { get; set; }
        public DbSet<PreProDetail> PreProDetails { get; set; }
        public DbSet<SessionSchoolClass> SessionSchoolClasses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=cs4rsa.db");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Keyword>()
            .HasOne(k => k.Discipline)
            .WithMany(d => d.Keywords)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SessionDetail>()
                .HasOne(sessionDetail => sessionDetail.Session)
                .WithMany(session => session.SessionDetails)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SessionSchoolClass>()
                .HasOne(sessionSchoolClass => sessionSchoolClass.SessionDetail)
                .WithMany(sessionDetail => sessionDetail.SessionSchoolClasses)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Keyword>()
                .HasOne(keyword => keyword.Discipline)
                .WithMany(discipline => discipline.Keywords)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ParProDetail>()
                .HasKey(x => new { x.ProgramSubjectId, x.PreParSubjectId });
            modelBuilder.Entity<PreProDetail>()
                .HasKey(x => new { x.ProgramSubjectId, x.PreParSubjectId });
        }
    }
}
