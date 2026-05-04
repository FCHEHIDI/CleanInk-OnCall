namespace CleanInk.OnCall.Shared.Fhir;

/// <summary>
/// FHIR R4 ContactPoint — details for technology-mediated contact (phone, email, fax, etc.).
/// </summary>
/// <param name="System">Type of contact: "phone" | "fax" | "email" | "pager" | "url" | "sms" | "other".</param>
/// <param name="Value">The actual contact value (number, address…).</param>
/// <param name="Use">Purpose: "home" | "work" | "temp" | "old" | "mobile".</param>
/// <param name="Rank">Preferred ranking (1 = highest priority).</param>
public sealed record ContactPoint(
    string System,
    string Value,
    string? Use = null,
    int? Rank = null)
{
    public static class Systems
    {
        public const string Phone = "phone";
        public const string Email = "email";
        public const string Fax   = "fax";
        public const string Sms   = "sms";
        public const string Url   = "url";
    }

    public static ContactPoint Phone(string number, string use = "work") =>
        new(Systems.Phone, number, use, 1);

    public static ContactPoint Mobile(string number) =>
        new(Systems.Phone, number, "mobile", 2);

    public static ContactPoint Email(string address, string use = "work") =>
        new(Systems.Email, address, use);
}
