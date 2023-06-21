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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-GD9867B\\SQLEXPRESS;Initial Catalog=Cwebiz;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC07BAA96B98");

            entity.ToTable("Course");

            entity.HasIndex(e => e.SemesterInfor, "UQ__Course__A8232F1CE6149AC7").IsUnique();

            entity.HasIndex(e => e.SemesterValue, "UQ__Course__B1935875C5A2A570").IsUnique();

            entity.HasIndex(e => e.YearInfor, "UQ__Course__BF305DB9FD524035").IsUnique();

            entity.HasIndex(e => e.YearValue, "UQ__Course__C795062E554BDB75").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Curricul__3214EC076A032CA3");

            entity.ToTable("Curriculum");

            entity.HasIndex(e => e.Name, "UQ__Curricul__737584F6FFE6999A").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<CwebizUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CwebizUs__3214EC075967B322");

            entity.ToTable("CwebizUser");

            entity.HasIndex(e => e.Username, "UQ__CwebizUs__536C85E42DA8036D").IsUnique();

            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Discipli__3214EC07A2C64F45");

            entity.ToTable("Discipline");

            entity.HasIndex(e => e.Name, "UQ__Discipli__737584F62353C105").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("CourseId_Course_Id");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Keyword__3214EC07C2C56EE6");

            entity.ToTable("Keyword");

            entity.HasIndex(e => e.CourseId, "UQ__Keyword__C92D71A628DDC0CB").IsUnique();

            entity.HasIndex(e => e.Color, "UQ__Keyword__E11D38453C474CAE").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.Keyword1).HasMaxLength(100);
            entity.Property(e => e.SubjectName).HasMaxLength(100);

            entity.HasOne(d => d.Discipline).WithMany(p => p.Keywords)
                .HasForeignKey(d => d.DisciplineId)
                .HasConstraintName("DisciplineId_Discipline_Id");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99181D6CBD");

            entity.ToTable("Student");

            entity.HasIndex(e => e.CwebizUserId, "CwebizUserId_CwebizUser_Id").IsUnique();

            entity.HasIndex(e => e.SpecialString, "UQ__Student__5DB99036F5E7C2C8").IsUnique();

            entity.HasIndex(e => e.Cmnd, "UQ__Student__B1BC1171C04079FF").IsUnique();

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

            entity.HasOne(d => d.CwebizUser).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.CwebizUserId)
                .HasConstraintName("FK__Student__CwebizU__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
