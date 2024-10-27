using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forms.Api.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class NewOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormEntity_Users_UserId",
                table: "FormEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionEntity_FormEntity_FormId",
                table: "QuestionEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseEntity_QuestionEntity_QuestionId",
                table: "ResponseEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseEntity_Users_UserId",
                table: "ResponseEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResponseEntity",
                table: "ResponseEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionEntity",
                table: "QuestionEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormEntity",
                table: "FormEntity");

            migrationBuilder.RenameTable(
                name: "ResponseEntity",
                newName: "Responses");

            migrationBuilder.RenameTable(
                name: "QuestionEntity",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "FormEntity",
                newName: "Forms");

            migrationBuilder.RenameIndex(
                name: "IX_ResponseEntity_UserId",
                table: "Responses",
                newName: "IX_Responses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ResponseEntity_QuestionId",
                table: "Responses",
                newName: "IX_Responses_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionEntity_FormId",
                table: "Questions",
                newName: "IX_Questions_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_FormEntity_UserId",
                table: "Forms",
                newName: "IX_Forms_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserResponse",
                table: "Responses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Responses",
                table: "Responses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Forms",
                table: "Forms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Users_UserId",
                table: "Forms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Forms_FormId",
                table: "Questions",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Questions_QuestionId",
                table: "Responses",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Users_UserId",
                table: "Responses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Users_UserId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Forms_FormId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Questions_QuestionId",
                table: "Responses");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Users_UserId",
                table: "Responses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Responses",
                table: "Responses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Forms",
                table: "Forms");

            migrationBuilder.RenameTable(
                name: "Responses",
                newName: "ResponseEntity");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "QuestionEntity");

            migrationBuilder.RenameTable(
                name: "Forms",
                newName: "FormEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_UserId",
                table: "ResponseEntity",
                newName: "IX_ResponseEntity_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_QuestionId",
                table: "ResponseEntity",
                newName: "IX_ResponseEntity_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_FormId",
                table: "QuestionEntity",
                newName: "IX_QuestionEntity_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_UserId",
                table: "FormEntity",
                newName: "IX_FormEntity_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserResponse",
                table: "ResponseEntity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResponseEntity",
                table: "ResponseEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionEntity",
                table: "QuestionEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormEntity",
                table: "FormEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormEntity_Users_UserId",
                table: "FormEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionEntity_FormEntity_FormId",
                table: "QuestionEntity",
                column: "FormId",
                principalTable: "FormEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseEntity_QuestionEntity_QuestionId",
                table: "ResponseEntity",
                column: "QuestionId",
                principalTable: "QuestionEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseEntity_Users_UserId",
                table: "ResponseEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
