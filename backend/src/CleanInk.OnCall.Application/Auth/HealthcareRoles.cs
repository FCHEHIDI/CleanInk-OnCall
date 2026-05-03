namespace CleanInk.OnCall.Application.Auth;

/// <summary>
/// Defines the healthcare role constants used throughout the application.
///
/// Role hierarchy (high → low privilege):
/// Admin > Medecin > InfirmierDE > SecretaireMedicale > Patient
///
/// These map directly to JWT claims and Angular route guards.
/// In a full HDS implementation, roles would be backed by French healthcare
/// professional identifiers (RPPS for doctors, ADELI for nurses).
/// </summary>
public static class HealthcareRoles
{
    /// <summary>System administrator — full access, user management.</summary>
    public const string Admin = "Admin";

    /// <summary>Médecin — full clinical access, can close/escalate all calls.</summary>
    public const string Medecin = "Medecin";

    /// <summary>Infirmier(ère) Diplômé(e) d'État — clinical access, can create and manage calls.</summary>
    public const string InfirmierDE = "InfirmierDE";

    /// <summary>Secrétaire médicale — administrative access, billing, call creation.</summary>
    public const string SecretaireMedicale = "SecretaireMedicale";

    /// <summary>Patient — read-only access to their own records.</summary>
    public const string Patient = "Patient";

    /// <summary>All valid role names.</summary>
    public static readonly IReadOnlyList<string> All =
        [Admin, Medecin, InfirmierDE, SecretaireMedicale, Patient];

    /// <summary>Returns true if the given role string is a recognised healthcare role.</summary>
    public static bool IsValid(string role) =>
        All.Contains(role, StringComparer.OrdinalIgnoreCase);

    /// <summary>Roles that have clinical (patient-data) access.</summary>
    public static readonly string[] ClinicalRoles = [Admin, Medecin, InfirmierDE];

    /// <summary>Roles that can manage billing and invoices.</summary>
    public static readonly string[] BillingRoles = [Admin, SecretaireMedicale];

    /// <summary>Roles that can view audit logs.</summary>
    public static readonly string[] AuditRoles = [Admin];
}
