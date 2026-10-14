using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ToDoApp.Infrastructure.Persistence;

#nullable disable

namespace Todo.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(TodoDbContext))]
    [Migration("20261014150500_AddReminderSentAtToToDo")]
    public partial class AddReminderSentAtToToDo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderSentAt",
                table: "ToDos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSentAt",
                table: "ToDos");
        }
    }
}
