using cs4rsa_core.Constants;
using cs4rsa_core.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

namespace cs4rsa_core.Cs4rsaDatabase.DataProviders
{
    public class Cs4rsaDbContext : DbContext
    {
        public Cs4rsaDbContext() : base()
        {

        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<UserSchedule> Sessions { get; set; }
        public DbSet<ScheduleDetail> SessionDetails { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ProgramSubject> ProgramSubjects { get; set; }
        public DbSet<PreParSubject> PreParSubjects { get; set; }
        public DbSet<ParProDetail> ParProDetails { get; set; }
        public DbSet<PreProDetail> PreProDetails { get; set; }
        public DbSet<KeywordTeacher> KeywordTeachers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(VMConstants.DB_CONN);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Keyword>()
            .HasOne(k => k.Discipline)
            .WithMany(d => d.Keywords)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScheduleDetail>()
                .HasOne(sessionDetail => sessionDetail.UserSchedule)
                .WithMany(session => session.SessionDetails)
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
