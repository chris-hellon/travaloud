using System.Collections.ObjectModel;

namespace Travaloud.Shared.Authorization;

public static class TravaloudRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);
    public const string Guest = nameof(Guest);
    public const string TourManager = nameof(TourManager);
    public const string PropertyManager = nameof(PropertyManager);
    public const string Supplier = nameof(Supplier);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        Admin,
        Basic,
        Guest,
        TourManager,
        PropertyManager,
        Supplier
    });

    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}