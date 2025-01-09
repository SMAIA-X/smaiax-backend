using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMAIAXBackend.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class AddContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyRequest",
                schema: "domain");

            migrationBuilder.CreateTable(
                name: "Contract",
                schema: "domain",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    policyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Contract", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contract",
                schema: "domain");

            migrationBuilder.CreateTable(
                name: "PolicyRequest",
                schema: "domain",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    isAutomaticContractingEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    locationResolution = table.Column<string>(type: "text", nullable: false),
                    locations = table.Column<string>(type: "text", nullable: false),
                    maxHouseHoldSize = table.Column<int>(type: "integer", nullable: false),
                    maxPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    measurementResolution = table.Column<string>(type: "text", nullable: false),
                    minHouseHoldSize = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_PolicyRequest", x => x.id);
                });
        }
    }
}