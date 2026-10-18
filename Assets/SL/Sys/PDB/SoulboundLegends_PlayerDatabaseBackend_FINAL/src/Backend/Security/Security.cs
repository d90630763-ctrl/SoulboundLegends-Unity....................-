using System.Security.Claims;
namespace SoulboundLegends.PlayerDatabaseBackend.Security;
public sealed class SecurityOptions { public string? Authority {get;set;} public string? Audience {get;set;} public string[] AdminRoles {get;set;}=["admin","operations"] ; }
public interface IAuthorityContext { Guid AccountId{get;} string ActorId{get;} bool IsServer{get;} bool IsAdmin{get;} void RequirePlayer(Guid playerId); void RequireAdmin(); }
public sealed class JwtAuthorityContext(IHttpContextAccessor http,IConfiguration cfg): IAuthorityContext
{
    ClaimsPrincipal User=>http.HttpContext?.User ?? new ClaimsPrincipal();
    public Guid AccountId=>Guid.TryParse(User.FindFirstValue("account_id"),out var id)?id:Guid.Empty;
    public string ActorId=>User.FindFirstValue("sub")??"unknown";
    Guid PlayerId=>Guid.TryParse(User.FindFirstValue("player_id"),out var id)?id:Guid.Empty;
    public bool IsServer=>User.IsInRole("server") || User.HasClaim("actor_type","server");
    public bool IsAdmin=>User.IsInRole("admin")||User.IsInRole("operations");
    public void RequirePlayer(Guid id){if(IsServer)return; if(PlayerId==Guid.Empty||PlayerId!=id)throw new UnauthorizedAccessException("Player ownership check failed.");}
    public void RequireAdmin(){if(!IsAdmin)throw new UnauthorizedAccessException("Administrative authorization required.");}
}
public sealed class ConcurrencyException(string message):Exception(message);
