using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ToDoApp.Infrastructure.Persistence;

#nullable disable

namespace Todo.Infrastructure.Migrations;

[DbContext(typeof(TodoDbContext))]
[Migration("20261015163800_AddPasswordResetRequestCooldown")]
public partial class AddPasswordResetRequestCooldown : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "PasswordResetRequestedAt",
            table: "Users",
            type: "TEXT",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PasswordResetRequestedAt",
            table: "Users");
    }
}
