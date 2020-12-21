using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SqlApp.Migrations
{
    public partial class RunScript : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // OR FROM FILE
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sql", @"instnwnd.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }
    }
}
