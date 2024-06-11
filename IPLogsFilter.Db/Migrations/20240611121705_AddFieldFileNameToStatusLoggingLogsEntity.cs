using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPLogsFilter.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldFileNameToStatusLoggingLogsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "status_logging_logs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_name",
                table: "status_logging_logs");
        }
    }
}
