﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace front_to_back.Migrations
{
    public partial class FeatureWorkComponentAndPhotos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeaturedWorkComponent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedWorkComponent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureWorkComponentPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    FeaturedWorkComponentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureWorkComponentPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureWorkComponentPhotos_FeaturedWorkComponent_FeaturedWorkComponentId",
                        column: x => x.FeaturedWorkComponentId,
                        principalTable: "FeaturedWorkComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureWorkComponentPhotos_FeaturedWorkComponentId",
                table: "FeatureWorkComponentPhotos",
                column: "FeaturedWorkComponentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeatureWorkComponentPhotos");

            migrationBuilder.DropTable(
                name: "FeaturedWorkComponent");
        }
    }
}
