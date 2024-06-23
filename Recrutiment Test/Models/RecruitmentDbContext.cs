using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Recrutiment_Test.Models;

public partial class RecruitmentDbContext : DbContext
{
    public RecruitmentDbContext()
    {
    }

    public RecruitmentDbContext(DbContextOptions<RecruitmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<ApprovalRequest> ApprovalRequests { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=RecruitmentDB; Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property(e => e.PasswordHashed)
                .HasMaxLength(44)
                .IsFixedLength();
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_AppUsers_Employees");
        });

        modelBuilder.Entity<ApprovalRequest>(entity =>
        {
            entity.ToTable("Approval Requests");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.LeaveRequest).HasColumnName("Leave Request");

            entity.HasOne(d => d.ApproverNavigation).WithMany(p => p.ApprovalRequests)
                .HasForeignKey(d => d.Approver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approval Requests_Employees");

            entity.HasOne(d => d.LeaveRequestNavigation).WithMany(p => p.ApprovalRequests)
                .HasForeignKey(d => d.LeaveRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approval Requests_Leave Requests");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Employee");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.HasOne(d => d.PeoplePartnerNavigation).WithMany(p => p.InversePeoplePartnerNavigation)
                .HasForeignKey(d => d.PeoplePartner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Employees");

            entity.HasMany(d => d.Projects).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeProjectAssignment",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EmployeeProjectAssignments_Projects"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EmployeeProjectAssignments_Employees"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "ProjectId");
                        j.ToTable("EmployeeProjectAssignments");
                        j.IndexerProperty<int>("EmployeeId").HasColumnName("EmployeeID");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("ProjectID");
                    });
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Leave Request");

            entity.ToTable("Leave Requests");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AbsenceReason).HasColumnName("Absence Reason");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.EndDate).HasColumnName("End Date");
            entity.Property(e => e.StartDate).HasColumnName("Start Date");

            entity.HasOne(d => d.EmployeeNavigation).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.Employee)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Leave Requests_Employees");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.EndDate).HasColumnName("End Date");
            entity.Property(e => e.ProjectManager).HasColumnName("Project Manager");
            entity.Property(e => e.ProjectType).HasColumnName("Project Type");
            entity.Property(e => e.StartDate).HasColumnName("Start Date");

            entity.HasOne(d => d.ProjectManagerNavigation).WithMany(p => p.ProjectsNavigation)
                .HasForeignKey(d => d.ProjectManager)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Employees");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
