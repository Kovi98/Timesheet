using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Db.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    DocumentSource = table.Column<byte[]>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    DocumentName = table.Column<string>(maxLength: 50, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStorage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Finance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    HourReward = table.Column<decimal>(type: "decimal(19, 2)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    PaymentDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ToPay = table.Column<bool>(nullable: false),
                    PaymentXML = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Surname = table.Column<string>(maxLength: 50, nullable: false),
                    DateBirth = table.Column<DateTime>(type: "date", nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Street = table.Column<string>(maxLength: 50, nullable: true),
                    HouseNumber = table.Column<string>(maxLength: 10, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    State = table.Column<string>(maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(fixedLength: true, maxLength: 5, nullable: true),
                    BankAccount = table.Column<string>(maxLength: 50, nullable: true),
                    BankCode = table.Column<string>(maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    HasTax = table.Column<bool>(nullable: false),
                    SectionId = table.Column<int>(nullable: false),
                    PaidFromId = table.Column<int>(nullable: false),
                    JobId = table.Column<int>(nullable: false),
                    IdentityDocument = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_Job",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Person_Finance",
                        column: x => x.PaidFromId,
                        principalTable: "Finance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Person_Section",
                        column: x => x.SectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Timesheet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hours = table.Column<decimal>(type: "decimal(6, 2)", nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    JobId = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false),
                    DateTimeFrom = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateTimeTo = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    PaymentId = table.Column<int>(nullable: true),
                    Reward = table.Column<decimal>(type: "decimal(19, 2)", nullable: true),
                    Tax = table.Column<decimal>(type: "decimal(19, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timesheet_Job",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timesheet_Payment",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timesheet_Person",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Person_JobId",
                table: "Person",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_PaidFromId",
                table: "Person",
                column: "PaidFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_SectionId",
                table: "Person",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheet_JobId",
                table: "Timesheet",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheet_PaymentId",
                table: "Timesheet",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheet_PersonId",
                table: "Timesheet",
                column: "PersonId");

            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW [dbo].[RewardSummary]
                AS
                SELECT        dbo.Person.Id AS PersonId, SUM(dbo.Timesheet.Hours) AS Hours, SUM(dbo.Timesheet.Reward) AS Reward, dbo.Person.HasTax, SUM(dbo.Timesheet.Tax) AS Tax, YEAR(dbo.Timesheet.CreateTime) AS CreateDateTimeYear, 
                                         MONTH(dbo.Timesheet.CreateTime) AS CreateDateTimeMonth, dbo.Timesheet.PaymentId AS PaymentId, CASE dbo.Timesheet.PaymentId WHEN NULL THEN NULL WHEN 0 THEN NULL 
                                         ELSE dbo.Payment.PaymentDateTime END AS PaymentDateTime, ROW_NUMBER() OVER (ORDER BY 
                                             (SELECT        1)) AS Id
                FROM            dbo.Timesheet INNER JOIN
                                         dbo.Person ON dbo.Timesheet.PersonId = dbo.Person.Id INNER JOIN
                                         dbo.Job ON dbo.Job.Id = dbo.Timesheet.JobId LEFT OUTER JOIN
                                         dbo.Payment ON dbo.Timesheet.PaymentId = dbo.Payment.Id
                GROUP BY dbo.Person.Id, YEAR(dbo.Timesheet.CreateTime), MONTH(dbo.Timesheet.CreateTime), dbo.Person.HasTax, dbo.Timesheet.PaymentId, CASE dbo.Timesheet.PaymentId WHEN NULL THEN NULL WHEN 0 THEN NULL 
                                         ELSE dbo.Payment.PaymentDateTime END
                GO
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentStorage");

            migrationBuilder.DropTable(
                name: "Timesheet");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "Finance");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.Sql(@"
                DROP VIEW [dbo].[RewardSummary];
            ");
        }
    }
}
