using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanInk.OnCall.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignUsersToFhirDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropIndex(
                name: "ix_patients_last_name",
                table: "patients");

            migrationBuilder.DropIndex(
                name: "ix_patients_nir_unique",
                table: "patients");

            migrationBuilder.DropIndex(
                name: "IX_calls_OperatorId",
                table: "calls");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "users");

            migrationBuilder.DropColumn(
                name: "consent_status",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "email",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "nir_number",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "invoices");

            migrationBuilder.EnsureSchema(
                name: "cleanink_shared");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "patients",
                newName: "patients",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "invoices",
                newName: "invoices",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "calls",
                newName: "calls",
                newSchema: "public");

            migrationBuilder.RenameColumn(
                name: "LastName",
                schema: "public",
                table: "users",
                newName: "name_family");

            migrationBuilder.RenameColumn(
                name: "is_archived",
                schema: "public",
                table: "patients",
                newName: "is_pseudonymized");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                schema: "public",
                table: "invoices",
                newName: "IssuedAt");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "public",
                table: "invoices",
                newName: "PatientId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "invoices",
                newName: "DueAt");

            migrationBuilder.RenameColumn(
                name: "CallId",
                schema: "public",
                table: "invoices",
                newName: "ThirdPartyPayerClaimId");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_CustomerId",
                schema: "public",
                table: "invoices",
                newName: "IX_invoices_PatientId");

            migrationBuilder.RenameColumn(
                name: "OperatorId",
                schema: "public",
                table: "calls",
                newName: "EncounterId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "public",
                table: "calls",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_calls_CustomerId",
                schema: "public",
                table: "calls",
                newName: "IX_calls_CreatedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                schema: "public",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string[]>(
                name: "Name_Prefix",
                schema: "public",
                table: "users",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "Name_Suffix",
                schema: "public",
                table: "users",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_Use",
                schema: "public",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "name_given",
                schema: "public",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "consent_given",
                schema: "public",
                table: "patients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "consent_given_at",
                schema: "public",
                table: "patients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_points",
                schema: "public",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "gender",
                schema: "public",
                table: "patients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "identifiers",
                schema: "public",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "names",
                schema: "public",
                table: "patients",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nir_encrypted",
                schema: "public",
                table: "patients",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AmountCents",
                schema: "public",
                table: "invoices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<Guid>(
                name: "EncounterId",
                schema: "public",
                table: "invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                schema: "public",
                table: "invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                schema: "public",
                table: "invoices",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PractitionerId",
                schema: "public",
                table: "invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VatCents",
                schema: "public",
                table: "invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedPractitionerId",
                schema: "public",
                table: "calls",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                schema: "public",
                table: "calls",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "audit_events",
                schema: "cleanink_shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    outcome = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    actor_email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    actor_role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    resource_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_emergency_access = table.Column<bool>(type: "boolean", nullable: false),
                    emergency_justification = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    details = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "consents",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", maxLength: 30, nullable: false),
                    scope = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    consented_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    withdrawn_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    validity_period = table.Column<string>(type: "jsonb", nullable: false),
                    collected_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    collection_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "encounters",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifiers = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", maxLength: 30, nullable: false),
                    encounter_class = table.Column<string>(type: "jsonb", nullable: false),
                    encounter_type = table.Column<string>(type: "jsonb", nullable: true),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    practitioner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period = table.Column<string>(type: "jsonb", nullable: false),
                    reason_text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    reason_code = table.Column<string>(type: "jsonb", nullable: true),
                    clinical_note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encounters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "medical_documents",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encounter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    authored_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    storage_key = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medical_documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "cleanink_shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    FinessNumber = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId",
                schema: "public",
                table: "users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_patients_consent_given",
                schema: "public",
                table: "patients",
                column: "consent_given");

            migrationBuilder.CreateIndex(
                name: "IX_calls_AssignedPractitionerId",
                schema: "public",
                table: "calls",
                column: "AssignedPractitionerId");

            migrationBuilder.CreateIndex(
                name: "ix_audit_actor_id",
                schema: "cleanink_shared",
                table: "audit_events",
                column: "actor_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_recorded_at",
                schema: "cleanink_shared",
                table: "audit_events",
                column: "recorded_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_resource",
                schema: "cleanink_shared",
                table: "audit_events",
                columns: new[] { "resource_type", "resource_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_tenant_id",
                schema: "cleanink_shared",
                table: "audit_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_consents_patient_id",
                schema: "public",
                table: "consents",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_consents_status",
                schema: "public",
                table: "consents",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_encounters_patient_id",
                schema: "public",
                table: "encounters",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_encounters_practitioner_id",
                schema: "public",
                table: "encounters",
                column: "practitioner_id");

            migrationBuilder.CreateIndex(
                name: "IX_encounters_status",
                schema: "public",
                table: "encounters",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_medical_documents_patient_id",
                schema: "public",
                table: "medical_documents",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_medical_documents_patient_id_is_deleted",
                schema: "public",
                table: "medical_documents",
                columns: new[] { "patient_id", "is_deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_events",
                schema: "cleanink_shared");

            migrationBuilder.DropTable(
                name: "consents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "encounters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "medical_documents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "cleanink_shared");

            migrationBuilder.DropIndex(
                name: "IX_users_TenantId",
                schema: "public",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_patients_consent_given",
                schema: "public",
                table: "patients");

            migrationBuilder.DropIndex(
                name: "IX_calls_AssignedPractitionerId",
                schema: "public",
                table: "calls");

            migrationBuilder.DropColumn(
                name: "Name_Prefix",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Name_Suffix",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Name_Use",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "name_given",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "consent_given",
                schema: "public",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "consent_given_at",
                schema: "public",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "contact_points",
                schema: "public",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "gender",
                schema: "public",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "identifiers",
                schema: "public",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "names",
                schema: "public",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "nir_encrypted",
                schema: "public",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "EncounterId",
                schema: "public",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                schema: "public",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                schema: "public",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "PractitionerId",
                schema: "public",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "VatCents",
                schema: "public",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "AssignedPractitionerId",
                schema: "public",
                table: "calls");

            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "public",
                table: "calls");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "public",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "patients",
                schema: "public",
                newName: "patients");

            migrationBuilder.RenameTable(
                name: "invoices",
                schema: "public",
                newName: "invoices");

            migrationBuilder.RenameTable(
                name: "calls",
                schema: "public",
                newName: "calls");

            migrationBuilder.RenameColumn(
                name: "name_family",
                table: "users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "is_pseudonymized",
                table: "patients",
                newName: "is_archived");

            migrationBuilder.RenameColumn(
                name: "ThirdPartyPayerClaimId",
                table: "invoices",
                newName: "CallId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "invoices",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "IssuedAt",
                table: "invoices",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "DueAt",
                table: "invoices",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_PatientId",
                table: "invoices",
                newName: "IX_invoices_CustomerId");

            migrationBuilder.RenameColumn(
                name: "EncounterId",
                table: "calls",
                newName: "OperatorId");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "calls",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_calls_CreatedByUserId",
                table: "calls",
                newName: "IX_calls_CustomerId");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "users",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "consent_status",
                table: "patients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "patients",
                type: "character varying(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nir_number",
                table: "patients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "patients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AmountCents",
                table: "invoices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "invoices",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    actor_email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    old_values = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_patients_last_name",
                table: "patients",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "ix_patients_nir_unique",
                table: "patients",
                column: "nir_number",
                unique: true,
                filter: "nir_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_calls_OperatorId",
                table: "calls",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "ix_audit_actor_id",
                table: "audit_logs",
                column: "actor_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_entity",
                table: "audit_logs",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_occurred_at",
                table: "audit_logs",
                column: "occurred_at");
        }
    }
}
