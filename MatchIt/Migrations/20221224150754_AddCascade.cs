using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchIt.Migrations
{
    public partial class AddCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchingStudents_Courses_CourseId",
                table: "MatchingStudents");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchingStudents_Courses_CourseId",
                table: "MatchingStudents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchingStudents_Courses_CourseId",
                table: "MatchingStudents");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchingStudents_Courses_CourseId",
                table: "MatchingStudents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
