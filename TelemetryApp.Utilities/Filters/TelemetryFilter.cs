using TelemetryApp.Api.Dto.Exceptions;

namespace TelemetryApp.Utilities.Filters;

public class TelemetryFilter
{
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();
    public string[] AllowedRoutes { get; set; } = Array.Empty<string>();
    
    public string[] ForbiddenMethods { get; set; } = Array.Empty<string>();
    public string[] ForbiddenRoutes { get; set; } = Array.Empty<string>();

    public bool Filter(string method, string route)
    {
        return FilterMethod(method) || FilterRoute(route);
    }

    private bool FilterMethod(string method)
    {
        return Filter(method, allowedMethods, forbiddenMethods);
    }

    private bool FilterRoute(string route)
    {
        return Filter(route, allowedRoutes, forbiddenRoutes);
    }

    private static bool Filter(string value, IReadOnlySet<string> allowed, IReadOnlySet<string> forbidden)
    {
        return !allowed.Contains(value) && forbidden.Contains(value);
    }

    internal void ValidateRestrictions()
    {
        var methodsIntersection = AllowedMethods.Intersect(ForbiddenMethods).ToArray();
        if (methodsIntersection.Any())
        {
            throw new TelemetryFilterValidationException(methodsIntersection, nameof(AllowedMethods), nameof(ForbiddenMethods));
        }

        var routesIntersection = AllowedRoutes.Intersect(ForbiddenRoutes).ToArray();
        if (routesIntersection.Any())
        {
            throw new TelemetryFilterValidationException(routesIntersection, nameof(AllowedRoutes), nameof(ForbiddenRoutes));
        }
    }

    internal void BuildFilters()
    {
        allowedMethods = new HashSet<string>(AllowedMethods);
        allowedRoutes = new HashSet<string>(AllowedRoutes);
        forbiddenMethods = new HashSet<string>(ForbiddenMethods);
        forbiddenRoutes = new HashSet<string>(ForbiddenRoutes);
    }

    private HashSet<string> allowedMethods = new();
    private HashSet<string> allowedRoutes = new();
    private HashSet<string> forbiddenMethods = new();
    private HashSet<string> forbiddenRoutes = new();
}