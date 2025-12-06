using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class InitWithShema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "przetrwaj");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "przetrwaj",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "przetrwaj",
                columns: table => new
                {
                    IdCategory = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.IdCategory);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                schema: "przetrwaj",
                columns: table => new
                {
                    IdRegion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.IdRegion);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "przetrwaj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "przetrwaj",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IdRegion = table.Column<int>(type: "integer", nullable: false),
                    Banned = table.Column<bool>(type: "boolean", nullable: false),
                    BanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BanReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    BannedById = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Regions_IdRegion",
                        column: x => x.IdRegion,
                        principalSchema: "przetrwaj",
                        principalTable: "Regions",
                        principalColumn: "IdRegion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "przetrwaj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "przetrwaj",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "przetrwaj",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "przetrwaj",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "przetrwaj",
                columns: table => new
                {
                    IdPost = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    IdCategory = table.Column<int>(type: "integer", nullable: false),
                    IdRegion = table.Column<int>(type: "integer", nullable: false),
                    IdAutor = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.IdPost);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_IdAutor",
                        column: x => x.IdAutor,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_IdCategory",
                        column: x => x.IdCategory,
                        principalSchema: "przetrwaj",
                        principalTable: "Categories",
                        principalColumn: "IdCategory",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Regions_IdRegion",
                        column: x => x.IdRegion,
                        principalSchema: "przetrwaj",
                        principalTable: "Regions",
                        principalColumn: "IdRegion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                schema: "przetrwaj",
                columns: table => new
                {
                    IdAttachment = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IdPost = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AlternateDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.IdAttachment);
                    table.ForeignKey(
                        name: "FK_Attachments_Posts_IdPost",
                        column: x => x.IdPost,
                        principalSchema: "przetrwaj",
                        principalTable: "Posts",
                        principalColumn: "IdPost",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "przetrwaj",
                columns: table => new
                {
                    IdComment = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IdPost = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IdAutor = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.IdComment);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_IdAutor",
                        column: x => x.IdAutor,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_IdPost",
                        column: x => x.IdPost,
                        principalSchema: "przetrwaj",
                        principalTable: "Posts",
                        principalColumn: "IdPost",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                schema: "przetrwaj",
                columns: table => new
                {
                    IdVote = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IdPost = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IdUser = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.IdVote);
                    table.ForeignKey(
                        name: "FK_Votes_AspNetUsers_IdUser",
                        column: x => x.IdUser,
                        principalSchema: "przetrwaj",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votes_Posts_IdPost",
                        column: x => x.IdPost,
                        principalSchema: "przetrwaj",
                        principalTable: "Posts",
                        principalColumn: "IdPost",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "przetrwaj",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "przetrwaj",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "przetrwaj",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "przetrwaj",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "przetrwaj",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "przetrwaj",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdRegion",
                schema: "przetrwaj",
                table: "AspNetUsers",
                column: "IdRegion");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "przetrwaj",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_IdPost",
                schema: "przetrwaj",
                table: "Attachments",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IdAutor",
                schema: "przetrwaj",
                table: "Comments",
                column: "IdAutor");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IdPost",
                schema: "przetrwaj",
                table: "Comments",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IdAutor",
                schema: "przetrwaj",
                table: "Posts",
                column: "IdAutor");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IdCategory",
                schema: "przetrwaj",
                table: "Posts",
                column: "IdCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IdRegion",
                schema: "przetrwaj",
                table: "Posts",
                column: "IdRegion");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_IdPost_IdUser",
                schema: "przetrwaj",
                table: "Votes",
                columns: new[] { "IdPost", "IdUser" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_IdUser",
                schema: "przetrwaj",
                table: "Votes",
                column: "IdUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "Attachments",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "Comments",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "Votes",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "przetrwaj");

            migrationBuilder.DropTable(
                name: "Regions",
                schema: "przetrwaj");
        }
    }
}
