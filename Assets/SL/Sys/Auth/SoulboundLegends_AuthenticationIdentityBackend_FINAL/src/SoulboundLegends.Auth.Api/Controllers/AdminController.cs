using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SoulboundLegends.Auth.Api.Services;

namespace SoulboundLegends.Auth.Api.Controllers;

[ApiController, Authorize(Policy = "Admin"), Route("api/v1/admin")]
public sealed class AdminController(AdminService admin) : ControllerBase
{
    [HttpGet("accounts/{accountId:guid}")]
    public async Task<IActionResult> Account(Guid accountId, CancellationToken ct)
    {
        var account = await admin.FindAccountAsync(accountId, ct);
        return account == null ? NotFound() : Ok(new
        {
            account.Id, account.DisplayEmail, account.Status, account.CreatedAt, account.LastLoginAt, account.Roles
        });
    }

    [HttpGet("accounts/{accountId:guid}/sessions")]
    public async Task<IActionResult> Sessions(Guid accountId, CancellationToken ct)
        => Ok(await admin.SessionsAsync(accountId, ct));

    [HttpGet("accounts/{accountId:guid}/security-events")]
    public async Task<IActionResult> SecurityEvents(Guid accountId, CancellationToken ct)
        => Ok(await admin.SecurityEventsAsync(accountId, ct));

    [HttpPost("accounts/{accountId:guid}/force-logout")]
    public async Task<IActionResult> ForceLogout(Guid accountId, [FromQuery] string reason, CancellationToken ct)
    {
        await admin.ForceLogoutAsync(accountId, reason, Guid.Parse(User.FindFirstValue("sub")!), ct);
        return NoContent();
    }

    [HttpPost("accounts/{accountId:guid}/suspension")]
    public async Task<IActionResult> Suspension(Guid accountId, [FromQuery] bool suspended, CancellationToken ct)
    {
        await admin.SetSuspendedAsync(accountId, suspended, Guid.Parse(User.FindFirstValue("sub")!), ct);
        return NoContent();
    }
}
