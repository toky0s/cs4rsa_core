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
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC07827F8E6D");

            entity.ToTable("Course");

            entity.HasIndex(e => e.SemesterInfor, "UQ__Course__A8232F1CC304E843").IsUnique();

            entity.HasIndex(e => e.SemesterValue, "UQ__Course__B1935875E0A61725").IsUnique();

            entity.HasIndex(e => e.YearInfor, "UQ__Course__BF305DB9C28F85D0").IsUnique();

            entity.HasIndex(e => e.YearValue, "UQ__Course__C795062EDB1A78F2").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Curricul__3214EC075856D3BD");

            entity.ToTable("Curriculum");

            entity.HasIndex(e => e.Name, "UQ__Curricul__737584F629AB6FF7").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<CwebizUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CwebizUs__3214EC075B6533E4");

            entity.ToTable("CwebizUser");

            entity.HasIndex(e => e.Username, "UQ__CwebizUs__536C85E406BB3377").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Discipli__3214EC076FE6023E");

            entity.ToTable("Discipline");

            entity.HasIndex(e => e.Name, "UQ__Discipli__737584F6E69FDECF").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("CourseId_Course_Id");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Keyword__3214EC07E8F05640");

            entity.ToTable("Keyword");

            entity.HasIndex(e => e.CourseId, "UQ__Keyword__C92D71A65EFDAEA0").IsUnique();

            entity.HasIndex(e => e.Color, "UQ__Keyword__E11D3845B4319AE1").IsUnique();

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
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99450EFDF4");

            entity.ToTable("Student");

            entity.HasIndex(e => e.SpecialString, "UQ__Student__5DB990369640D2C5").IsUnique();

            entity.HasIndex(e => e.Cmnd, "UQ__Student__B1BC1171FE3B7668").IsUnique();

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
