using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IPLogsFilter.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddFiltredLogsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "filtred_logs_id",
                table: "log_records",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "filtred_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    log_record_id = table.Column<int>(type: "integer", nullable: false),
                    count_log_records = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_filtred_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_filtred_logs_log_records_log_record_id",
                        column: x => x.log_record_id,
                        principalTable: "log_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_filtred_logs_log_record_id",
                table: "filtred_logs",
                column: "log_record_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "filtred_logs");

            migrationBuilder.DropColumn(
                name: "filtred_logs_id",
                table: "log_records");
        }
    }
}
