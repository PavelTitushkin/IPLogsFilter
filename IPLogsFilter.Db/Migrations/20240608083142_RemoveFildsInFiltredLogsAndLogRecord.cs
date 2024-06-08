using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPLogsFilter.Db.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFildsInFiltredLogsAndLogRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_filtred_logs_log_records_log_record_id",
                table: "filtred_logs");

            migrationBuilder.DropIndex(
                name: "ix_filtred_logs_log_record_id",
                table: "filtred_logs");

            migrationBuilder.DropColumn(
                name: "filtred_logs_id",
                table: "log_records");

            migrationBuilder.DropColumn(
                name: "log_record_id",
                table: "filtred_logs");

            migrationBuilder.AddColumn<string>(
                name: "log_record",
                table: "filtred_logs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "log_record",
                table: "filtred_logs");

            migrationBuilder.AddColumn<int>(
                name: "filtred_logs_id",
                table: "log_records",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "log_record_id",
                table: "filtred_logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_filtred_logs_log_record_id",
                table: "filtred_logs",
                column: "log_record_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_filtred_logs_log_records_log_record_id",
                table: "filtred_logs",
                column: "log_record_id",
                principalTable: "log_records",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
