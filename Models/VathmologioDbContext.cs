using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VathmologioMVC.Models;

public partial class VathmologioDbContext : DbContext
{
    public VathmologioDbContext()
    {
    }

    public VathmologioDbContext(DbContextOptions<VathmologioDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseHasStudent> CourseHasStudents { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Secretary> Secretaries { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=VathmologioDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.IdCourse).HasName("PK__course__C18577576F682840");

            entity.ToTable("course");

            entity.Property(e => e.IdCourse).HasColumnName("idCOURSE");
            entity.Property(e => e.CourseSemester)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.ProfessorsAfm).HasColumnName("PROFESSORS_AFM");

            entity.HasOne(d => d.ProfessorsAfmNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.ProfessorsAfm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_course_professors_AFM");
        });

        modelBuilder.Entity<CourseHasStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__course_h__3214EC079187C239");

            entity.ToTable("course_has_students");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CourseIdCourse).HasColumnName("COURSE_idCOURSE");
            entity.Property(e => e.StudentsRegistrationNumber).HasColumnName("STUDENTS_RegistrationNumber");

            entity.HasOne(d => d.CourseIdCourseNavigation).WithMany(p => p.CourseHasStudents)
                .HasForeignKey(d => d.CourseIdCourse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_courseHasStudents_course_idCOURSE");

            entity.HasOne(d => d.StudentsRegistrationNumberNavigation).WithMany(p => p.CourseHasStudents)
                .HasForeignKey(d => d.StudentsRegistrationNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_courseHasStudents_students_RegistrationNumber");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Afm).HasName("PK__professo__C6906E6384649A2E");

            entity.ToTable("professors");

            entity.Property(e => e.Afm).HasColumnName("AFM");
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Professors)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_professors_users_username");
        });

        modelBuilder.Entity<Secretary>(entity =>
        {
            entity.HasKey(e => e.Phonenumber).HasName("PK__secretar__9FDCA5A61DF8A539");

            entity.ToTable("secretaries");

            entity.Property(e => e.Phonenumber).ValueGeneratedNever();
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Secretaries)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_secretaries_users_username");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.RegistrationNumber).HasName("PK__students__E8864603A395F7D7");

            entity.ToTable("students");

            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_students_users_username");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__users__F3DBC573DC2EC37C");

            entity.ToTable("users");

            entity.Property(e => e.Username)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
