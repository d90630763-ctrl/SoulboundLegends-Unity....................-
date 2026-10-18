using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulboundLegends.Auth.Api.Models;
using SoulboundLegends.Auth.Api.Services;
using System.Security.Claims;

namespace SoulboundLegends.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IAuthenticationService auth, IRecoveryService recovery) : ControllerBase
{
    private string? Ip => HttpContext.Connection.RemoteIpAddress?.ToString();
    private string? Ua => Request.Headers.UserAgent.ToString();

    [AllowAnonymous, HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
        => Ok(await Execute(() => auth.RegisterAsync(request, Ip, Ua, ct)));

    [AllowAnonymous, HttpPost("login/password")]
    public async Task<ActionResult<AuthResponse>> LoginPassword(PasswordLoginRequest request, CancellationToken ct)
        => Ok(await Execute(() => auth.LoginPasswordAsync(request, Ip, Ua, ct)));

    [AllowAnonymous, HttpPost("login/google")]
    public async Task<ActionResult<AuthResponse>> LoginGoogle(GoogleLoginRequest request, CancellationToken ct)
        => Ok(await Execute(() => auth.LoginGoogleAsync(request, Ip, Ua, ct)));

    [AllowAnonymous, HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken ct)
        => Ok(await Execute(() => auth.RefreshAsync(request, Ip, Ua, ct)));

    [Authorize, HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        await auth.LogoutAsync(AccountId(), SessionId(), request.AllDevices, ct);
        return NoContent();
    }

    [Authorize, HttpGet("session")]
    public async Task<ActionResult<SessionInfo>> Session(CancellationToken ct)
    {
        var s = await auth.GetSessionAsync(AccountId(), SessionId(), ct);
        return s == null ? NotFound() : Ok(s);
    }

    [Authorize, HttpGet("identities")]
    public async Task<ActionResult<IReadOnlyList<ExternalIdentityInfo>>> Identities(CancellationToken ct)
        => Ok(await auth.GetExternalIdentitiesAsync(AccountId(), ct));

    [Authorize, HttpPost("identities/google")]
    public async Task<IActionResult> LinkGoogle(LinkProviderRequest request, CancellationToken ct)
    {
        await auth.LinkGoogleAsync(AccountId(), request, ct);
        return NoContent();
    }

    [Authorize, HttpDelete("identities/{provider}")]
    public async Task<IActionResult> Unlink(string provider, CancellationToken ct)
    {
        await auth.UnlinkProviderAsync(AccountId(), provider, ct);
        return NoContent();
    }

    [AllowAnonymous, HttpPost("recovery/request")]
    public async Task<IActionResult> RecoveryRequest(RecoveryRequest request, CancellationToken ct)
    {
        await recovery.RequestAsync(request, ct);
        return Accepted();
    }

    [AllowAnonymous, HttpPost("recovery/complete")]
    public async Task<IActionResult> RecoveryComplete(CompleteRecoveryRequest request, CancellationToken ct)
    {
        await recovery.CompleteAsync(request, ct);
        return NoContent();
    }

    private Guid AccountId() => Guid.Parse(User.FindFirstValue("sub")!);
    private Guid SessionId() => Guid.Parse(User.FindFirst("sid")!.Value);

    private static async Task<T> Execute<T>(Func<Task<T>> f)
    {
        try { return await f(); }
        catch (UnauthorizedAccessException ex) { throw new AuthenticationHttpException(401, ex.Message); }
        catch (ArgumentException ex) { throw new AuthenticationHttpException(400, ex.Message); }
        catch (InvalidOperationException ex) { throw new AuthenticationHttpException(409, ex.Message); }
    }
}
public sealed class AuthenticationHttpException(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}
