using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

namespace Cs4rsa.Cs4rsaDatabase.DataProviders
{
    public class Cs4rsaDbContext : DbContext
    {
        public Cs4rsaDbContext() : base()
        {
            RSql = new RawSql(this);
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<UserSchedule> UserSchedules { get; set; }
        public DbSet<ScheduleDetail> ScheduleDetails { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<DbProgramSubject> DbProgramSubjects { get; set; }
        public DbSet<DbPreParSubject> DbPreParSubjects { get; set; }
        public DbSet<ParProDetail> ParProDetails { get; set; }
        public DbSet<PreProDetail> PreProDetails { get; set; }
        public DbSet<KeywordTeacher> KeywordTeachers { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public RawSql RSql { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(VmConstants.DbConnectionString);
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
