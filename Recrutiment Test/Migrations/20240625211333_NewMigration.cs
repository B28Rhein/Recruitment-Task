using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recrutiment_Test.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subdivision = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    PeoplePartner = table.Column<int>(type: "int", nullable: false),
                    OutOfOfficeBalance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Employees_Employees",
                        column: x => x.PeoplePartner,
                        principalTable: "Employees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHashed = table.Column<string>(type: "nchar(84)", fixedLength: true, maxLength: 84, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUsers_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Leave Requests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Employee = table.Column<int>(type: "int", nullable: false),
                    AbsenceReason = table.Column<int>(name: "Absence Reason", type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(name: "Start Date", type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(name: "End Date", type: "date", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leave Request", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Leave Requests_Employees",
                        column: x => x.Employee,
                        principalTable: "Employees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectType = table.Column<int>(name: "Project Type", type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(name: "Start Date", type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(name: "End Date", type: "date", nullable: true),
                    ProjectManager = table.Column<int>(name: "Project Manager", type: "int", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Projects_Employees",
                        column: x => x.ProjectManager,
                        principalTable: "Employees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Approval Requests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Approver = table.Column<int>(type: "int", nullable: false),
                    LeaveRequest = table.Column<int>(name: "Leave Request", type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval Requests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Approval Requests_Employees",
                        column: x => x.Approver,
                        principalTable: "Employees",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Approval Requests_Leave Requests",
                        column: x => x.LeaveRequest,
                        principalTable: "Leave Requests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProjectAssignments",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProjectAssignments", x => new { x.EmployeeID, x.ProjectID });
                    table.ForeignKey(
                        name: "FK_EmployeeProjectAssignments_Employees",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EmployeeProjectAssignments_Projects",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approval Requests_Approver",
                table: "Approval Requests",
                column: "Approver");

            migrationBuilder.CreateIndex(
                name: "IX_Approval Requests_Leave Request",
                table: "Approval Requests",
                column: "Leave Request");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_EmployeeId",
                table: "AppUsers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjectAssignments_ProjectID",
                table: "EmployeeProjectAssignments",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PeoplePartner",
                table: "Employees",
                column: "PeoplePartner");

            migrationBuilder.CreateIndex(
                name: "IX_Leave Requests_Employee",
                table: "Leave Requests",
                column: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Project Manager",
                table: "Projects",
                column: "Project Manager");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approval Requests");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "EmployeeProjectAssignments");

            migrationBuilder.DropTable(
                name: "Leave Requests");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
