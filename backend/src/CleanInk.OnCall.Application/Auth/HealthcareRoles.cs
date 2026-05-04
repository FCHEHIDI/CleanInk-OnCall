namespace CleanInk.OnCall.Application.Auth;

/// <summary>
/// Application RBAC role constants — 10 roles covering all healthcare RevOps personas.
///
/// Role hierarchy (high → low privilege):
///   Admin → DirecteurMedical → Medecin / Infirmier → SecretaireMedicale / ManagerCallCenter
///   → AgentCallCenter / AutoriteExterne → Patient / PartenaireExterne
///
/// Domain invariants enforced here (applied in authorization policies):
/// 1. Tenant isolation — users never cross tenant boundaries regardless of role.
/// 2. Emergency clinical access (Medecin only) — must produce EmergencyAccessAuditEvent.
/// 3. AuditEvent is append-only — no role has Update/Delete permission on it.
/// 4. PartenaireExterne uses API key + secret, not JWT.
/// 5. No self-elevation — a user cannot grant themselves a higher role.
///
/// These constants map 1:1 to JWT "role" claims and Angular route guards.
/// </summary>
public static class AppRoles
{
    // ── Staff roles ──────────────────────────────────────────────────────────

    /// <summary>System administrator — full access, user management, tenant provisioning.</summary>
    public const string Admin = "Admin";

    /// <summary>Directeur médical — global clinical oversight, KPI dashboard, AI reports.</summary>
    public const string DirecteurMedical = "DirecteurMedical";

    /// <summary>Médecin — full clinical access, prescriptions, teleconsultation, emergency override.</summary>
    public const string Medecin = "Medecin";

    /// <summary>Infirmier(ère) — clinical access (read + triage), call assignment, vital signs.</summary>
    public const string Infirmier = "Infirmier";

    /// <summary>Secrétaire médicale — scheduling, billing, administrative tasks.</summary>
    public const string SecretaireMedicale = "SecretaireMedicale";

    /// <summary>Agent call center — call creation, triage, basic patient lookup.</summary>
    public const string AgentCallCenter = "AgentCallCenter";

    /// <summary>Manager call center — call center oversight, queue management, KPIs.</summary>
    public const string ManagerCallCenter = "ManagerCallCenter";

    /// <summary>Autorité externe — SAMU, pompiers, police. Limited scoped access.</summary>
    public const string AutoriteExterne = "AutoriteExterne";

    // ── Non-staff roles ──────────────────────────────────────────────────────

    /// <summary>Patient — access to own records via patient portal only.</summary>
    public const string Patient = "Patient";

    /// <summary>Partenaire externe — API key service account (ambulance, lab, pharmacy).</summary>
    public const string PartenaireExterne = "PartenaireExterne";

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>All valid role names.</summary>
    public static readonly IReadOnlyList<string> All =
    [
        Admin, DirecteurMedical, Medecin, Infirmier,
        SecretaireMedicale, AgentCallCenter, ManagerCallCenter,
        AutoriteExterne, Patient, PartenaireExterne
    ];

    /// <summary>Returns true if the given role string is a recognised application role.</summary>
    public static bool IsValid(string role) =>
        All.Contains(role, StringComparer.OrdinalIgnoreCase);

    // ── Policy groups ────────────────────────────────────────────────────────

    /// <summary>Roles with full clinical (patient-data) read+write access.</summary>
    public static readonly string[] ClinicalRoles = [Admin, DirecteurMedical, Medecin, Infirmier];

    /// <summary>Roles authorized to prescribe or document clinical acts.</summary>
    public static readonly string[] PrescriptionRoles = [Admin, Medecin];

    /// <summary>Roles with billing and invoice access.</summary>
    public static readonly string[] BillingRoles = [Admin, SecretaireMedicale, DirecteurMedical];

    /// <summary>Roles with scheduling access.</summary>
    public static readonly string[] SchedulingRoles =
        [Admin, Medecin, Infirmier, SecretaireMedicale, DirecteurMedical];

    /// <summary>Roles that can create or manage calls.</summary>
    public static readonly string[] CallCenterRoles =
        [Admin, AgentCallCenter, ManagerCallCenter, Infirmier, Medecin];

    /// <summary>Roles that can escalate or close calls.</summary>
    public static readonly string[] EscalationRoles = [Admin, Medecin, ManagerCallCenter];

    /// <summary>Roles with read access to compliance audit logs.</summary>
    public static readonly string[] AuditRoles = [Admin, DirecteurMedical];

    /// <summary>Roles allowed to manage staff (HR).</summary>
    public static readonly string[] HrRoles = [Admin, DirecteurMedical];
}

// Backward-compatibility alias — do not use in new code.
[Obsolete("Use AppRoles instead of HealthcareRoles.")]
public static class HealthcareRoles
{
    public const string Admin             = AppRoles.Admin;
    public const string Medecin           = AppRoles.Medecin;
    public const string InfirmierDE       = AppRoles.Infirmier;
    public const string SecretaireMedicale = AppRoles.SecretaireMedicale;
    public const string Patient           = AppRoles.Patient;
    public static readonly string[] ClinicalRoles = AppRoles.ClinicalRoles;
    public static readonly string[] BillingRoles  = AppRoles.BillingRoles;
    public static readonly string[] AuditRoles    = AppRoles.AuditRoles;
    public static bool IsValid(string role) => AppRoles.IsValid(role);
}
