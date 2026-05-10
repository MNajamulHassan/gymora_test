using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymora.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentPlanId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanExpiryDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanStartDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MembershipPlans",
                columns: table => new
                {
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPlans", x => x.PlanId);
                    table.ForeignKey(
                        name: "FK_MembershipPlans_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CurrentPlanId",
                table: "AspNetUsers",
                column: "CurrentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipPlans_TenantId_PlanName",
                table: "MembershipPlans",
                columns: new[] { "TenantId", "PlanName" });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MembershipPlans_CurrentPlanId",
                table: "AspNetUsers",
                column: "CurrentPlanId",
                principalTable: "MembershipPlans",
                principalColumn: "PlanId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MembershipPlans_CurrentPlanId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MembershipPlans");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CurrentPlanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentPlanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlanExpiryDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlanStartDate",
                table: "AspNetUsers");
        }
    }
}
