﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Recrutiment_Test.Models;

#nullable disable

namespace Recrutiment_Test.Migrations
{
    [DbContext(typeof(RecruitmentDbContext))]
    [Migration("20240625211333_NewMigration")]
    partial class NewMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EmployeeProjectAssignment", b =>
                {
                    b.Property<int>("EmployeeId")
                        .HasColumnType("int")
                        .HasColumnName("EmployeeID");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int")
                        .HasColumnName("ProjectID");

                    b.HasKey("EmployeeId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("EmployeeProjectAssignments", (string)null);
                });

            modelBuilder.Entity("Recrutiment_Test.Models.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHashed")
                        .IsRequired()
                        .HasMaxLength(84)
                        .HasColumnType("nchar(84)")
                        .IsFixedLength();

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.ApprovalRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Approver")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<int>("LeaveRequest")
                        .HasColumnType("int")
                        .HasColumnName("Leave Request");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Approver");

                    b.HasIndex("LeaveRequest");

                    b.ToTable("Approval Requests", (string)null);
                });

            modelBuilder.Entity("Recrutiment_Test.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OutOfOfficeBalance")
                        .HasColumnType("int");

                    b.Property<int>("PeoplePartner")
                        .HasColumnType("int");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<int>("Subdivision")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_Employee");

                    b.HasIndex("PeoplePartner");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.LeaveRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AbsenceReason")
                        .HasColumnType("int")
                        .HasColumnName("Absence Reason");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<int>("Employee")
                        .HasColumnType("int");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date")
                        .HasColumnName("End Date");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date")
                        .HasColumnName("Start Date");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_Leave Request");

                    b.HasIndex("Employee");

                    b.ToTable("Leave Requests", (string)null);
                });

            modelBuilder.Entity("Recrutiment_Test.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date")
                        .HasColumnName("End Date");

                    b.Property<int>("ProjectManager")
                        .HasColumnType("int")
                        .HasColumnName("Project Manager");

                    b.Property<int>("ProjectType")
                        .HasColumnType("int")
                        .HasColumnName("Project Type");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date")
                        .HasColumnName("Start Date");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ProjectManager");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("EmployeeProjectAssignment", b =>
                {
                    b.HasOne("Recrutiment_Test.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .IsRequired()
                        .HasConstraintName("FK_EmployeeProjectAssignments_Employees");

                    b.HasOne("Recrutiment_Test.Models.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .IsRequired()
                        .HasConstraintName("FK_EmployeeProjectAssignments_Projects");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.AppUser", b =>
                {
                    b.HasOne("Recrutiment_Test.Models.Employee", "Employee")
                        .WithMany("AppUsers")
                        .HasForeignKey("EmployeeId")
                        .HasConstraintName("FK_AppUsers_Employees");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.ApprovalRequest", b =>
                {
                    b.HasOne("Recrutiment_Test.Models.Employee", "ApproverNavigation")
                        .WithMany("ApprovalRequests")
                        .HasForeignKey("Approver")
                        .IsRequired()
                        .HasConstraintName("FK_Approval Requests_Employees");

                    b.HasOne("Recrutiment_Test.Models.LeaveRequest", "LeaveRequestNavigation")
                        .WithMany("ApprovalRequests")
                        .HasForeignKey("LeaveRequest")
                        .IsRequired()
                        .HasConstraintName("FK_Approval Requests_Leave Requests");

                    b.Navigation("ApproverNavigation");

                    b.Navigation("LeaveRequestNavigation");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.Employee", b =>
                {
                    b.HasOne("Recrutiment_Test.Models.Employee", "PeoplePartnerNavigation")
                        .WithMany("InversePeoplePartnerNavigation")
                        .HasForeignKey("PeoplePartner")
                        .IsRequired()
                        .HasConstraintName("FK_Employees_Employees");

                    b.Navigation("PeoplePartnerNavigation");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.LeaveRequest", b =>
                {
                    b.HasOne("Recrutiment_Test.Models.Employee", "EmployeeNavigation")
                        .WithMany("LeaveRequests")
                        .HasForeignKey("Employee")
                        .IsRequired()
                        .HasConstraintName("FK_Leave Requests_Employees");

                    b.Navigation("EmployeeNavigation");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.Project", b =>
                {
                    b.HasOne("Recrutiment_Test.Models.Employee", "ProjectManagerNavigation")
                        .WithMany("ProjectsNavigation")
                        .HasForeignKey("ProjectManager")
                        .IsRequired()
                        .HasConstraintName("FK_Projects_Employees");

                    b.Navigation("ProjectManagerNavigation");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.Employee", b =>
                {
                    b.Navigation("AppUsers");

                    b.Navigation("ApprovalRequests");

                    b.Navigation("InversePeoplePartnerNavigation");

                    b.Navigation("LeaveRequests");

                    b.Navigation("ProjectsNavigation");
                });

            modelBuilder.Entity("Recrutiment_Test.Models.LeaveRequest", b =>
                {
                    b.Navigation("ApprovalRequests");
                });
#pragma warning restore 612, 618
        }
    }
}