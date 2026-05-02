namespace QuickPOS.Core;

public interface ICurrentUserProvider
{
    string? Username { get; }
}

/// <summary>
/// Singleton that holds the currently authenticated username.
/// Set by AuthenticationService on login/logout.
/// Read by AuditableInterceptor without touching the DbContext.
/// </summary>
public class CurrentUserProvider : ICurrentUserProvider
{
    public string? Username { get; set; }
}
