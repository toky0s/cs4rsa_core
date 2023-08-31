using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CwebizAPI.Share.Database.Models;

public partial class CwebizContext : DbContext
{
    public CwebizContext()
    {
    }

    public CwebizContext(DbContextOptions<CwebizContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Curriculum> Curricula { get; set; }

    public virtual DbSet<CwebizUser> CwebizUsers { get; set; }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Keyword> Keywords { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-GD9867B\\SQLEXPRESS;Initial Catalog=Cwebiz;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC073E7BD466");

            entity.ToTable("Course");

            entity.HasIndex(e => e.SemesterInfor, "UQ__Course__A8232F1CDCA58AEA").IsUnique();

            entity.HasIndex(e => e.SemesterValue, "UQ__Course__B193587587D8D464").IsUnique();

            entity.HasIndex(e => e.YearInfor, "UQ__Course__BF305DB94D6A8E4F").IsUnique();

            entity.HasIndex(e => e.YearValue, "UQ__Course__C795062E1A4E1F81").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SemesterInfor).HasMaxLength(100);
            entity.Property(e => e.SemesterValue)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.YearInfor).HasMaxLength(100);
            entity.Property(e => e.YearValue)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Curriculum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Curricul__3214EC07B012BBA7");

            entity.ToTable("Curriculum");

            entity.HasIndex(e => e.Name, "UQ__Curricul__737584F6E17F1CCE").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<CwebizUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CwebizUs__3214EC07740D1C0A");

            entity.ToTable("CwebizUser");

            entity.HasIndex(e => e.Username, "UQ__CwebizUs__536C85E4219E5A14").IsUnique();

            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.StudentId)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Discipli__3214EC07C34B7950");

            entity.ToTable("Discipline");

            entity.HasIndex(e => e.Name, "UQ__Discipli__737584F6E686203C").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("CourseId_Course_Id");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Keyword__3214EC07091E2C2B");

            entity.ToTable("Keyword");

            entity.HasIndex(e => e.CourseId, "UQ__Keyword__C92D71A69FD0E9EE").IsUnique();

            entity.HasIndex(e => e.Color, "UQ__Keyword__E11D3845395AF1C8").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.CourseId)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Keyword1)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SubjectName).HasMaxLength(100);

            entity.HasOne(d => d.Discipline).WithMany(p => p.Keywords)
                .HasForeignKey(d => d.DisciplineId)
                .HasConstraintName("DisciplineId_Discipline_Id");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99FD193B05");

            entity.ToTable("Student");

            entity.HasIndex(e => e.SpecialString, "UQ__Student__5DB990368B868527").IsUnique();

            entity.HasIndex(e => e.Cmnd, "UQ__Student__B1BC117194910E61").IsUnique();

            entity.Property(e => e.StudentId)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.AvatarImgPath).IsUnicode(false);
            entity.Property(e => e.BirthDay).HasColumnType("date");
            entity.Property(e => e.Cmnd)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.SpecialString)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Curriculum).WithMany(p => p.Students)
                .HasForeignKey(d => d.CurriculumId)
                .HasConstraintName("CurriculumId_Curriculum_Id");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__EDF25964B28B7817");

            entity.ToTable("Teacher");

            entity.Property(e => e.TeacherId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Degree).HasMaxLength(100);
            entity.Property(e => e.Form).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Place).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.Sex).HasMaxLength(3);
            entity.Property(e => e.Subject).HasMaxLength(100);
            entity.Property(e => e.WorkUnit).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
