using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ToDoApp.Infrastructure.Persistence;

#nullable disable

namespace Todo.Infrastructure.Migrations;

[DbContext(typeof(TodoDbContext))]
[Migration("20261014160000_AddPasswordResetTokens")]
public partial class AddPasswordResetTokens : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PasswordResetTokenHash",
            table: "Users",
            type: "TEXT",
            maxLength: 64,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "PasswordResetTokenExpiresAt",
            table: "Users",
            type: "TEXT",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Users_PasswordResetTokenHash",
            table: "Users",
            column: "PasswordResetTokenHash",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_PasswordResetTokenHash",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "PasswordResetTokenHash",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "PasswordResetTokenExpiresAt",
            table: "Users");
    }
}
