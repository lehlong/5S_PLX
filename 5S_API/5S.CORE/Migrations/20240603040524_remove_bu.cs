﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLX5S.CORE.Migrations
{
    /// <inheritdoc />
    public partial class remove_bu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_BU_MODULE_ATTACHMENT");

            migrationBuilder.DropTable(
                name: "T_BU_ATTACHMENT");

            migrationBuilder.CreateTable(
                name: "T_CM_ATTACHMENT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "nvarchar2(255)", nullable: false),
                    URL = table.Column<string>(type: "varchar2(255)", nullable: false),
                    EXTENSION = table.Column<string>(type: "varchar2(255)", nullable: false),
                    SIZE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TYPE = table.Column<string>(type: "varchar2(255)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    CREATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    UPDATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    CREATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UPDATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CM_ATTACHMENT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_CM_MODULE_ATTACHMENT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    REFERENCE_ID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    MODULE_TYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ATTACHMENT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    CREATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    UPDATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    CREATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UPDATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CM_MODULE_ATTACHMENT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_CM_MODULE_ATTACHMENT_T_CM_ATTACHMENT_ATTACHMENT_ID",
                        column: x => x.ATTACHMENT_ID,
                        principalTable: "T_CM_ATTACHMENT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_CM_MODULE_ATTACHMENT_ATTACHMENT_ID",
                table: "T_CM_MODULE_ATTACHMENT",
                column: "ATTACHMENT_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_CM_MODULE_ATTACHMENT");

            migrationBuilder.DropTable(
                name: "T_CM_ATTACHMENT");

            migrationBuilder.CreateTable(
                name: "T_BU_ATTACHMENT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CREATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    CREATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    EXTENSION = table.Column<string>(type: "varchar2(255)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    NAME = table.Column<string>(type: "nvarchar2(255)", nullable: false),
                    SIZE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TYPE = table.Column<string>(type: "varchar2(255)", nullable: false),
                    UPDATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    UPDATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    URL = table.Column<string>(type: "varchar2(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_BU_ATTACHMENT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_BU_MODULE_ATTACHMENT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ATTACHMENT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CREATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    CREATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    MODULE_TYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    REFERENCE_ID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    UPDATE_BY = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    UPDATE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_BU_MODULE_ATTACHMENT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_BU_MODULE_ATTACHMENT_T_BU_ATTACHMENT_ATTACHMENT_ID",
                        column: x => x.ATTACHMENT_ID,
                        principalTable: "T_BU_ATTACHMENT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_BU_MODULE_ATTACHMENT_ATTACHMENT_ID",
                table: "T_BU_MODULE_ATTACHMENT",
                column: "ATTACHMENT_ID");
        }
    }
}
