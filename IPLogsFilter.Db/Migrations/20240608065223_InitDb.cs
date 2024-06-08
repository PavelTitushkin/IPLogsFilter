using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IPLogsFilter.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "log_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    request_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    application_name = table.Column<string>(type: "text", nullable: false),
                    stage = table.Column<string>(type: "text", nullable: false),
                    client_ip_address = table.Column<IPAddress>(type: "inet", nullable: false),
                    client_name = table.Column<string>(type: "text", nullable: false),
                    client_version = table.Column<string>(type: "text", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    method = table.Column<string>(type: "text", nullable: false),
                    status_code = table.Column<string>(type: "text", nullable: false),
                    status_mesage = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    content_length = table.Column<string>(type: "text", nullable: false),
                    execution_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    memroy_usage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_log_records", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "log_records");
        }
    }
}
