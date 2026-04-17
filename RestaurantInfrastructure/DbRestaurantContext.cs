using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RestaurantInfrastructure.Models;

using RestDomain.Models;
public partial class DbRestaurantContext : DbContext
{
    public DbRestaurantContext()
    {
    }

    public DbRestaurantContext(DbContextOptions<DbRestaurantContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bonuse> Bonuses { get; set; }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftsType> ShiftsTypes { get; set; }

    public virtual DbSet<TrainingProgress> TrainingProgresses { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Schedule;Username=berezhna;Password=student");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bonuse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Bonuses_pkey");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Employee).WithMany(p => p.Bonuses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Bonuses_EmployeeID_fkey");

            entity.HasOne(d => d.Manager).WithMany(p => p.Bonuses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Bonuses_ManagerID_fkey");
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Employers_pkey");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("Instructor_pkey");
            // Прибираємо ValueGeneratedOnAdd для FK/PK ключа, він має братися від Employer
            entity.Property(e => e.EmployeeId).ValueGeneratedNever();

            entity.HasOne(d => d.Employee)
                .WithOne(p => p.Instructor)
                .HasForeignKey<Instructor>(d => d.EmployeeId) // Вказуємо явно
                .OnDelete(DeleteBehavior.Cascade)             // ДОДАНО КАСКАД
                .HasConstraintName("Instructor_EmployeeID_fkey");

            entity.HasOne(d => d.Manager).WithMany(p => p.Instructors)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Instructor_ManagerID_fkey");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("Managers_pkey");
            entity.Property(e => e.EmployeeId).ValueGeneratedNever();

            entity.HasOne(d => d.Employee)
                .WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.EmployeeId) // Вказуємо явно
                .OnDelete(DeleteBehavior.Cascade)             // ДОДАНО КАСКАД
                .HasConstraintName("Managers_EmloyeeID_fkey");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Shifts_pkey");
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();

            entity.HasOne(d => d.Employee).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Shifts_EmpoyeeID_fkey");

            entity.HasOne(d => d.ShiftType).WithMany(p => p.Shifts).HasConstraintName("Shifts_ShiftTypeID_fkey");
        });

        modelBuilder.Entity<ShiftsType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ShiftsTypes_pkey");
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TrainingProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TrainingProgress_pkey");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.InstuctorDNavigation).WithMany(p => p.TrainingProgresses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("TrainingProgress_InstuctorD_fkey");

            entity.HasOne(d => d.Worker).WithMany(p => p.TrainingProgresses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("TrainingProgress_WorkerID_fkey");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("Workers_pkey");
            entity.Property(e => e.EmployeeId).ValueGeneratedNever();

            entity.HasOne(d => d.Employee)
                .WithOne(p => p.Worker)
                .HasForeignKey<Worker>(d => d.EmployeeId) // Вказуємо явно
                .OnDelete(DeleteBehavior.Cascade)           // ДОДАНО КАСКАД
                .HasConstraintName("Workers_EmpoyeeID_fkey");

            entity.HasOne(d => d.Instructor).WithMany(p => p.Workers).HasConstraintName("Workers_InstructorID_fkey");
            entity.HasOne(d => d.Manager).WithMany(p => p.Workers).HasConstraintName("Workers_ManagerID_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
