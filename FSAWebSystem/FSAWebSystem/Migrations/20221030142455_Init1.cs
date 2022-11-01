﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Andromedas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UUStock = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ITThisWeek = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RRACT13Wk = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WeekCover = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Andromedas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Trade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannerName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BottomPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvgNormalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgBottomPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgActualPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinActualPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Gap = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottomPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FSACalendarHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FSACalendarHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FSADocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FSADocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ITrusts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SumIntransit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SumStock = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SumFinalRpp = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DistStock = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITrusts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryProduct = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSADocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProposalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BannerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rephase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedAt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleUnilevers",
                columns: table => new
                {
                    RoleUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUnilevers", x => x.RoleUnileverId);
                });

            migrationBuilder.CreateTable(
                name: "ULICalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ULICalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyBucketHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    BannerPlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DispatchConsume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyBucketHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSADocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FSACalendarDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FSACalendarHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FSACalendarDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FSACalendarDetails_FSACalendarHeaders_FSACalendarHeaderId",
                        column: x => x.FSACalendarHeaderId,
                        principalTable: "FSACalendarHeaders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BannerPlants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CDM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KAM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSADocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerPlants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannerPlants_Banners_BannerId",
                        column: x => x.BannerId,
                        principalTable: "Banners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannerPlants_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SKUs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCMap = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DescriptionMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSADocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKUs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SKUs_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuRoleUnilever",
                columns: table => new
                {
                    MenusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleUnileversRoleUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRoleUnilever", x => new { x.MenusId, x.RoleUnileversRoleUnileverId });
                    table.ForeignKey(
                        name: "FK_MenuRoleUnilever_Menus_MenusId",
                        column: x => x.MenusId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuRoleUnilever_RoleUnilevers_RoleUnileversRoleUnileverId",
                        column: x => x.RoleUnileversRoleUnileverId,
                        principalTable: "RoleUnilevers",
                        principalColumn: "RoleUnileverId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersUnilever",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WLId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FSADocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersUnilever", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersUnilever_RoleUnilevers_RoleUnileverId",
                        column: x => x.RoleUnileverId,
                        principalTable: "RoleUnilevers",
                        principalColumn: "RoleUnileverId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ULICalendarDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    ULICalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ULICalendarDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                        column: x => x.ULICalendarId,
                        principalTable: "ULICalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBuckets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SWF2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SWF3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerPlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlantContribution = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RatingRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TCT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyTarget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FSADocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBuckets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyBuckets_BannerPlants_BannerPlantId",
                        column: x => x.BannerPlantId,
                        principalTable: "BannerPlants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonthlyBuckets_FSADocuments_FSADocumentId",
                        column: x => x.FSADocumentId,
                        principalTable: "FSADocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyBuckets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BannerPlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantContribution = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyBucket = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RatingRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BucketWeek1 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BucketWeek2 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BucketWeek3 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BucketWeek4 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BucketWeek5 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValidBJ = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemFSA = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DispatchConsume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyBuckets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyBuckets_BannerPlants_BannerPlantId",
                        column: x => x.BannerPlantId,
                        principalTable: "BannerPlants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BannerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KAM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CDM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rephase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    IsWaitingApproval = table.Column<bool>(type: "bit", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposals_Banners_BannerId",
                        column: x => x.BannerId,
                        principalTable: "Banners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposals_SKUs_SkuId",
                        column: x => x.SkuId,
                        principalTable: "SKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BannerPlantUserUnilever",
                columns: table => new
                {
                    BannerPlantsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerPlantUserUnilever", x => new { x.BannerPlantsId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_BannerPlantUserUnilever_BannerPlants_BannerPlantsId",
                        column: x => x.BannerPlantsId,
                        principalTable: "BannerPlants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannerPlantUserUnilever_UsersUnilever_UserUnileversId",
                        column: x => x.UserUnileversId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategoryUserUnilever",
                columns: table => new
                {
                    ProductCategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategoryUserUnilever", x => new { x.ProductCategoriesId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_ProductCategoryUserUnilever_ProductCategories_ProductCategoriesId",
                        column: x => x.ProductCategoriesId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategoryUserUnilever_UsersUnilever_UserUnileversId",
                        column: x => x.UserUnileversId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SKUUserUnilever",
                columns: table => new
                {
                    SKUsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKUUserUnilever", x => new { x.SKUsId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_SKUUserUnilever_SKUs_SKUsId",
                        column: x => x.SKUsId,
                        principalTable: "SKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SKUUserUnilever_UsersUnilever_UserUnileversId",
                        column: x => x.UserUnileversId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approvals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ProposalType = table.Column<int>(type: "int", nullable: false),
                    ApprovalNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ApproverWL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approvals_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approvals_UsersUnilever_UserUnileverId",
                        column: x => x.UserUnileverId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProposalDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BannerPlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WeeklyBucketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActualRephase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlantContribution = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsTarget = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalDetails_BannerPlants_BannerPlantId",
                        column: x => x.BannerPlantId,
                        principalTable: "BannerPlants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProposalDetails_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Rephase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlantContribution = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalDetail_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDetail_ApprovalId",
                table: "ApprovalDetail",
                column: "ApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ProposalId",
                table: "Approvals",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_UserUnileverId",
                table: "Approvals",
                column: "UserUnileverId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BannerPlants_BannerId",
                table: "BannerPlants",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerPlants_PlantId",
                table: "BannerPlants",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerPlantUserUnilever_UserUnileversId",
                table: "BannerPlantUserUnilever",
                column: "UserUnileversId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_BannerName",
                table: "Banners",
                column: "BannerName");

            migrationBuilder.CreateIndex(
                name: "IX_FSACalendarDetails_FSACalendarHeaderId",
                table: "FSACalendarDetails",
                column: "FSACalendarHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuRoleUnilever_RoleUnileversRoleUnileverId",
                table: "MenuRoleUnilever",
                column: "RoleUnileversRoleUnileverId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBuckets_BannerPlantId",
                table: "MonthlyBuckets",
                column: "BannerPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBuckets_FSADocumentId",
                table: "MonthlyBuckets",
                column: "FSADocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryUserUnilever_UserUnileversId",
                table: "ProductCategoryUserUnilever",
                column: "UserUnileversId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalDetails_BannerPlantId",
                table: "ProposalDetails",
                column: "BannerPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalDetails_ProposalId",
                table: "ProposalDetails",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_BannerId",
                table: "Proposals",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_SkuId",
                table: "Proposals",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_SKUs_PCMap",
                table: "SKUs",
                column: "PCMap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SKUs_ProductCategoryId",
                table: "SKUs",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SKUUserUnilever_UserUnileversId",
                table: "SKUUserUnilever",
                column: "UserUnileversId");

            migrationBuilder.CreateIndex(
                name: "IX_ULICalendarDetails_ULICalendarId",
                table: "ULICalendarDetails",
                column: "ULICalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersUnilever_RoleUnileverId",
                table: "UsersUnilever",
                column: "RoleUnileverId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyBuckets_BannerPlantId",
                table: "WeeklyBuckets",
                column: "BannerPlantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Andromedas");

            migrationBuilder.DropTable(
                name: "ApprovalDetail");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BannerPlantUserUnilever");

            migrationBuilder.DropTable(
                name: "BottomPrices");

            migrationBuilder.DropTable(
                name: "FSACalendarDetails");

            migrationBuilder.DropTable(
                name: "ITrusts");

            migrationBuilder.DropTable(
                name: "MenuRoleUnilever");

            migrationBuilder.DropTable(
                name: "MonthlyBuckets");

            migrationBuilder.DropTable(
                name: "ProductCategoryUserUnilever");

            migrationBuilder.DropTable(
                name: "ProposalDetails");

            migrationBuilder.DropTable(
                name: "ProposalHistories");

            migrationBuilder.DropTable(
                name: "SKUUserUnilever");

            migrationBuilder.DropTable(
                name: "ULICalendarDetails");

            migrationBuilder.DropTable(
                name: "WeeklyBucketHistories");

            migrationBuilder.DropTable(
                name: "WeeklyBuckets");

            migrationBuilder.DropTable(
                name: "WorkLevels");

            migrationBuilder.DropTable(
                name: "Approvals");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FSACalendarHeaders");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "FSADocuments");

            migrationBuilder.DropTable(
                name: "ULICalendars");

            migrationBuilder.DropTable(
                name: "BannerPlants");

            migrationBuilder.DropTable(
                name: "Proposals");

            migrationBuilder.DropTable(
                name: "UsersUnilever");

            migrationBuilder.DropTable(
                name: "Plants");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "SKUs");

            migrationBuilder.DropTable(
                name: "RoleUnilevers");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}