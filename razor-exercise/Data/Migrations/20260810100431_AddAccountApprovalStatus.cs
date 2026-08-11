using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace razor_exercise.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            // Preserve access for accounts that already existed before this approval workflow.
            migrationBuilder.Sql("UPDATE \"AspNetUsers\" SET \"ApprovalStatus\" = 1;");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovalStatus",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "AspNetUsers");
        }
    }
}
