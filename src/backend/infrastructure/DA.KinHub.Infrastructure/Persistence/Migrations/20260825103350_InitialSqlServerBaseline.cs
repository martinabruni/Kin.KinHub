using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DA.KinHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServerBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.EnsureSchema(
                name: "kinlist");

            migrationBuilder.CreateTable(
                name: "application_users",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    external_issuer = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    external_object_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    inactive_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "kin_services",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    route = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_preconfigured = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kin_services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "families",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_by_application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    inactive_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_families", x => x.Id);
                    table.ForeignKey(
                        name: "FK_families_application_users_created_by_application_user_id",
                        column: x => x.created_by_application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "kin_service_localizations",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    kin_service_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    language = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kin_service_localizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kin_service_localizations_kin_services_kin_service_id",
                        column: x => x.kin_service_id,
                        principalSchema: "shared",
                        principalTable: "kin_services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "kinlist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    normalized_name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    created_by_application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    inactive_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                    table.UniqueConstraint("AK_categories_Id_family_id", x => new { x.Id, x.family_id });
                    table.ForeignKey(
                        name: "FK_categories_application_users_created_by_application_user_id",
                        column: x => x.created_by_application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_categories_families_family_id",
                        column: x => x.family_id,
                        principalSchema: "shared",
                        principalTable: "families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "family_invitations",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_by_application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    code_hmac = table.Column<byte[]>(type: "varbinary(32)", nullable: false),
                    hmac_key_version = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    consumed_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family_invitations", x => x.Id);
                    table.CheckConstraint("CK_family_invitations_consumed_after_created", "consumed_at IS NULL OR consumed_at >= created_at");
                    table.CheckConstraint("CK_family_invitations_expires_after_created", "expires_at > created_at");
                    table.CheckConstraint("CK_family_invitations_hmac_key_version_non_empty", "LEN(hmac_key_version) > 0");
                    table.CheckConstraint("CK_family_invitations_hmac_non_empty", "DATALENGTH(code_hmac) > 0");
                    table.CheckConstraint("CK_family_invitations_revoked_after_created", "revoked_at IS NULL OR revoked_at >= created_at");
                    table.ForeignKey(
                        name: "FK_family_invitations_application_users_created_by_application_user_id",
                        column: x => x.created_by_application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_family_invitations_families_family_id",
                        column: x => x.family_id,
                        principalSchema: "shared",
                        principalTable: "families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "family_kin_service_availabilities",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    kin_service_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family_kin_service_availabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_family_kin_service_availabilities_families_family_id",
                        column: x => x.family_id,
                        principalSchema: "shared",
                        principalTable: "families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_family_kin_service_availabilities_kin_services_kin_service_id",
                        column: x => x.kin_service_id,
                        principalSchema: "shared",
                        principalTable: "kin_services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "family_memberships",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    inactive_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family_memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_family_memberships_application_users_application_user_id",
                        column: x => x.application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_family_memberships_families_family_id",
                        column: x => x.family_id,
                        principalSchema: "shared",
                        principalTable: "families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registration_groups",
                schema: "kinlist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    recording_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_by_application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    inactive_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration_groups", x => x.Id);
                    table.UniqueConstraint("AK_registration_groups_Id_family_id", x => new { x.Id, x.family_id });
                    table.ForeignKey(
                        name: "FK_registration_groups_application_users_created_by_application_user_id",
                        column: x => x.created_by_application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registration_groups_families_family_id",
                        column: x => x.family_id,
                        principalSchema: "shared",
                        principalTable: "families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "items",
                schema: "kinlist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    registration_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    position_in_group = table.Column<int>(type: "int", nullable: false),
                    owner_application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    visibility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    modified_by_application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    completed_by_application_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    inactive_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    revision = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.Id);
                    table.UniqueConstraint("AK_items_Id_family_id", x => new { x.Id, x.family_id });
                    table.CheckConstraint("CK_items_position_in_group_non_negative", "position_in_group >= 0");
                    table.CheckConstraint("CK_items_revision_positive", "revision >= 1");
                    table.CheckConstraint("CK_items_status", "status IN ('Active', 'Completed')");
                    table.CheckConstraint("CK_items_visibility", "visibility IN ('Shared', 'Personal')");
                    table.ForeignKey(
                        name: "FK_items_application_users_completed_by_application_user_id",
                        column: x => x.completed_by_application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_application_users_modified_by_application_user_id",
                        column: x => x.modified_by_application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_application_users_owner_application_user_id",
                        column: x => x.owner_application_user_id,
                        principalSchema: "shared",
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_registration_groups_registration_group_id_family_id",
                        columns: x => new { x.registration_group_id, x.family_id },
                        principalSchema: "kinlist",
                        principalTable: "registration_groups",
                        principalColumns: new[] { "Id", "family_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_categories",
                schema: "kinlist",
                columns: table => new
                {
                    item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_categories", x => new { x.item_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_item_categories_categories_category_id_family_id",
                        columns: x => new { x.category_id, x.family_id },
                        principalSchema: "kinlist",
                        principalTable: "categories",
                        principalColumns: new[] { "Id", "family_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_categories_items_item_id_family_id",
                        columns: x => new { x.item_id, x.family_id },
                        principalSchema: "kinlist",
                        principalTable: "items",
                        principalColumns: new[] { "Id", "family_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "shared",
                table: "kin_services",
                columns: new[] { "Id", "created_at", "is_active", "is_preconfigured", "key", "route", "updated_at" },
                values: new object[] { new Guid("a5f1cb74-e8f7-4cdc-8d95-f1ad39090d18"), new DateTimeOffset(new DateTime(2026, 7, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, true, "kinlist", "/kinlist", null });

            migrationBuilder.InsertData(
                schema: "shared",
                table: "kin_service_localizations",
                columns: new[] { "Id", "created_at", "description", "kin_service_id", "language", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("8ec4ca56-9097-4d4d-8c88-cc9224d1e0d0"), new DateTimeOffset(new DateTime(2026, 7, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Shared list for the family.", new Guid("a5f1cb74-e8f7-4cdc-8d95-f1ad39090d18"), "en", "KinList", null },
                    { new Guid("fc4db75e-7813-4ee7-92b5-2ce17fd90518"), new DateTimeOffset(new DateTime(2026, 7, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Lista condivisa della famiglia.", new Guid("a5f1cb74-e8f7-4cdc-8d95-f1ad39090d18"), "it", "KinList", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_users_external_issuer_external_object_id",
                schema: "shared",
                table: "application_users",
                columns: new[] { "external_issuer", "external_object_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_created_by_application_user_id",
                schema: "kinlist",
                table: "categories",
                column: "created_by_application_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_family_id_normalized_name",
                schema: "kinlist",
                table: "categories",
                columns: new[] { "family_id", "normalized_name" },
                unique: true,
                filter: "inactive_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_families_created_by_application_user_id",
                schema: "shared",
                table: "families",
                column: "created_by_application_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_family_invitations_active_by_family_created_at_id",
                schema: "shared",
                table: "family_invitations",
                columns: new[] { "family_id", "created_at", "Id" },
                filter: "revoked_at IS NULL AND consumed_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_family_invitations_created_by_application_user_id",
                schema: "shared",
                table: "family_invitations",
                column: "created_by_application_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_family_kin_service_availabilities_family_id",
                schema: "shared",
                table: "family_kin_service_availabilities",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_family_kin_service_availabilities_family_id_kin_service_id",
                schema: "shared",
                table: "family_kin_service_availabilities",
                columns: new[] { "family_id", "kin_service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_family_kin_service_availabilities_kin_service_id",
                schema: "shared",
                table: "family_kin_service_availabilities",
                column: "kin_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_family_memberships_application_user_id_family_id",
                schema: "shared",
                table: "family_memberships",
                columns: new[] { "application_user_id", "family_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_family_memberships_application_user_id_family_id_inactive_at",
                schema: "shared",
                table: "family_memberships",
                columns: new[] { "application_user_id", "family_id", "inactive_at" });

            migrationBuilder.CreateIndex(
                name: "IX_family_memberships_family_id",
                schema: "shared",
                table: "family_memberships",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_family_memberships_single_active_user",
                schema: "shared",
                table: "family_memberships",
                column: "application_user_id",
                unique: true,
                filter: "inactive_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_category_id_family_id",
                schema: "kinlist",
                table: "item_categories",
                columns: new[] { "category_id", "family_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_family_id_category_id_item_id",
                schema: "kinlist",
                table: "item_categories",
                columns: new[] { "family_id", "category_id", "item_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_family_id_item_id",
                schema: "kinlist",
                table: "item_categories",
                columns: new[] { "family_id", "item_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_item_id_family_id",
                schema: "kinlist",
                table: "item_categories",
                columns: new[] { "item_id", "family_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_completed_by_application_user_id",
                schema: "kinlist",
                table: "items",
                column: "completed_by_application_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_items_modified_by_application_user_id",
                schema: "kinlist",
                table: "items",
                column: "modified_by_application_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_items_owner_application_user_id",
                schema: "kinlist",
                table: "items",
                column: "owner_application_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_items_personal_active",
                schema: "kinlist",
                table: "items",
                columns: new[] { "registration_group_id", "owner_application_user_id", "position_in_group", "Id" },
                filter: "inactive_at IS NULL AND status = 'Active' AND visibility = 'Personal'");

            migrationBuilder.CreateIndex(
                name: "IX_items_registration_group_id_family_id",
                schema: "kinlist",
                table: "items",
                columns: new[] { "registration_group_id", "family_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_registration_group_id_position_in_group",
                schema: "kinlist",
                table: "items",
                columns: new[] { "registration_group_id", "position_in_group" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_shared_active",
                schema: "kinlist",
                table: "items",
                columns: new[] { "registration_group_id", "position_in_group", "Id" },
                filter: "inactive_at IS NULL AND status = 'Active' AND visibility = 'Shared'");

            migrationBuilder.CreateIndex(
                name: "IX_kin_service_localizations_kin_service_id_language",
                schema: "shared",
                table: "kin_service_localizations",
                columns: new[] { "kin_service_id", "language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kin_services_key",
                schema: "shared",
                table: "kin_services",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kin_services_route",
                schema: "shared",
                table: "kin_services",
                column: "route",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_registration_groups_created_by_application_user_id",
                schema: "kinlist",
                table: "registration_groups",
                column: "created_by_application_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_registration_groups_family_id_created_at_Id",
                schema: "kinlist",
                table: "registration_groups",
                columns: new[] { "family_id", "created_at", "Id" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "IX_registration_groups_family_id_recording_id",
                schema: "kinlist",
                table: "registration_groups",
                columns: new[] { "family_id", "recording_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "family_invitations",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "family_kin_service_availabilities",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "family_memberships",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "item_categories",
                schema: "kinlist");

            migrationBuilder.DropTable(
                name: "kin_service_localizations",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "kinlist");

            migrationBuilder.DropTable(
                name: "items",
                schema: "kinlist");

            migrationBuilder.DropTable(
                name: "kin_services",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "registration_groups",
                schema: "kinlist");

            migrationBuilder.DropTable(
                name: "families",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "application_users",
                schema: "shared");
        }
    }
}
