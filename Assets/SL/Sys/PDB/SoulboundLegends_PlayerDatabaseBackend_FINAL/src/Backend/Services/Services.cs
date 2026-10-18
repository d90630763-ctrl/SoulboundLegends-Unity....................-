using System.Text.Json;
using System.Security;
using Dapper;
using SoulboundLegends.PlayerDatabaseBackend.Security;
namespace SoulboundLegends.PlayerDatabaseBackend.Services;
public sealed class PlayerRepository(IDatabaseProvider provider)
{
 public async Task<PlayerSnapshot?> GetSnapshotAsync(Guid playerId,CancellationToken ct){await using var c=await provider.OpenAsync(ct);return await c.QuerySingleOrDefaultAsync<PlayerSnapshot>(new CommandDefinition("select player_id as PlayerId,account_id as AccountId,character_id as CharacterId,character_name as CharacterName,gender as Gender,face as Face,hair as Hair,appearance_json as Appearance,level,xp,rank,version,updated_at as UpdatedAt, profile_json as Data from players where player_id=@playerId",new{playerId},cancellationToken:ct));}
}
public sealed class MutationService(IDatabaseProvider provider,ITransactionRunner tx,AuditService audit)
{
 public async Task<MutationResult> ApplyAsync(Guid playerId,MutationRequest req,IAuthorityContext auth,CancellationToken ct)
 {
  if(string.IsNullOrWhiteSpace(req.IdempotencyKey)||req.IdempotencyKey.Length>128)throw new ArgumentException("A bounded idempotency key is required.");
  return await tx.RunAsync(async(c,t)=>{
   var prior=await c.QuerySingleOrDefaultAsync<(string Key,long Version,string Result)>(new CommandDefinition("select idempotency_key Key,new_version Version,result_json Result from idempotency_keys where idempotency_key=@k and player_id=@p for update",new{k=req.IdempotencyKey,p=playerId},t));
   if(prior.Key is not null)return new MutationResult(prior.Key,prior.Version,JsonSerializer.Deserialize<object>(prior.Result));
   var row=await c.QuerySingleAsync<(long Version)>(new CommandDefinition("select version from players where player_id=@p for update",new{p=playerId},t));
   if(row.Version!=req.ExpectedVersion)throw new ConcurrencyException($"Expected version {req.ExpectedVersion}, actual {row.Version}.");
   var allowed=new[]{"profile.update","settings.update","progression.update","inventory.snapshot","skill.unlock","quest.progress","discovery.record","equipment.update","social.update"};
   if(!allowed.Contains(req.Operation,StringComparer.Ordinal))throw new ArgumentException("Unsupported mutation operation.");
   var next=row.Version+1;
   await c.ExecuteAsync(new CommandDefinition("update players set version=@next,updated_at=now(),profile_json=case when @op='profile.update' then cast(@payload as jsonb) else profile_json end where player_id=@p",new{next,op=req.Operation,p=playerId,payload=JsonSerializer.Serialize(req.Payload)},t));
   var result=JsonSerializer.Serialize(new{accepted=true,operation=req.Operation});
   await c.ExecuteAsync(new CommandDefinition("insert into idempotency_keys(idempotency_key,player_id,new_version,result_json) values(@k,@p,@v,cast(@r as jsonb))",new{k=req.IdempotencyKey,p=playerId,v=next,r=result},t));
   await audit.WriteAsync(c,t,auth.ActorId,playerId,"mutation",req.Operation,req.IdempotencyKey,ct);
   return new MutationResult(req.IdempotencyKey,next,new{accepted=true,operation=req.Operation});
  },ct);
 }
}
public sealed class AuditService
{
 public async Task WriteAsync(System.Data.IDbConnection c,System.Data.IDbTransaction t,string actor,Guid player,string category,string action,string correlation,CancellationToken ct)=>await c.ExecuteAsync(new CommandDefinition("insert into audit_log(actor_id,player_id,category,action,correlation_id) values(@actor,@player,@category,@action,@correlation)",new{actor,player,category,action,correlation},t,cancellationToken:ct));
}
public sealed class WorldStateService(ITransactionRunner tx)
{
 public Task AdvanceRiftAsync(Guid riftId,RiftState expected,RiftState next,Guid? cityId,CancellationToken ct)=>tx.RunAsync(async(c,t)=>{var valid=(expected,next) switch{(RiftState.Active,RiftState.Escalated)=>true,(RiftState.Escalated,RiftState.Merging)=>true,(RiftState.Merging,RiftState.Engulfed)=>true,(RiftState.Engulfed,RiftState.Cleared)=>true,(RiftState.Engulfed,RiftState.Failed)=>true,_=>false};if(!valid)throw new InvalidOperationException("Invalid Rift state transition.");var n=await c.ExecuteAsync(new CommandDefinition("update rifts set state=@next,version=version+1,updated_at=now() where rift_id=@id and state=@expected",new{id=riftId,expected=expected.ToString(),next=next.ToString()},t));if(n!=1)throw new SecurityException("Rift state changed concurrently.");if(next==RiftState.Failed&&cityId.HasValue)await c.ExecuteAsync(new CommandDefinition("update cities set destroyed=true,updated_at=now() where city_id=@city",new{city=cityId},t));return Task.CompletedTask;},ct);
}
public sealed class AdminService(IDatabaseProvider p)
{
 public async Task<object> InspectPlayerAsync(Guid id,IAuthorityContext a,CancellationToken ct){a.RequireAdmin();await using var c=await p.OpenAsync(ct);return await c.QuerySingleOrDefaultAsync("select * from players where player_id=@id",new{id});}
 public async Task<object> InspectTransactionsAsync(Guid id,IAuthorityContext a,CancellationToken ct){a.RequireAdmin();await using var c=await p.OpenAsync(ct);return (await c.QueryAsync("select * from economy_ledger where player_id=@id order by created_at desc limit 500",new{id})).ToList();}
 public async Task<object> InspectRiftAsync(Guid id,IAuthorityContext a,CancellationToken ct){a.RequireAdmin();await using var c=await p.OpenAsync(ct);return await c.QuerySingleOrDefaultAsync("select * from rifts where rift_id=@id",new{id});}
 public async Task<object> InspectRuinAsync(Guid id,IAuthorityContext a,CancellationToken ct){a.RequireAdmin();await using var c=await p.OpenAsync(ct);return await c.QuerySingleOrDefaultAsync("select * from supreme_ruins where ruin_id=@id",new{id});}
}

public sealed class EconomyService(ITransactionRunner tx, AuditService audit)
{
 public Task<decimal> ApplyAsync(Guid playerId,string currency,decimal delta,string reason,string idempotencyKey,long expectedVersion,IAuthorityContext auth,CancellationToken ct)=>tx.RunAsync(async(c,t)=>{
   if(string.IsNullOrWhiteSpace(currency)||currency.Length>64) throw new ArgumentException("Invalid currency.");
   if(string.IsNullOrWhiteSpace(idempotencyKey)||idempotencyKey.Length>128) throw new ArgumentException("Invalid idempotency key.");
   var prior=await c.QuerySingleOrDefaultAsync<decimal?>(new CommandDefinition("select balance_after from economy_ledger where idempotency_key=@k",new{k=idempotencyKey},t));
   if(prior.HasValue)return prior.Value;
   var row=await c.QuerySingleAsync<(decimal Balance,long Version)>(new CommandDefinition("select balance,version from economy_balances where player_id=@p and currency=@c for update",new{p=playerId,c=currency},t));
   if(row.Version!=expectedVersion) throw new ConcurrencyException("Economy balance changed concurrently.");
   var next=row.Balance+delta;
   if(next<0) throw new InvalidOperationException("Insufficient balance.");
   await c.ExecuteAsync(new CommandDefinition("update economy_balances set balance=@next,version=version+1,updated_at=now() where player_id=@p and currency=@c",new{next,p=playerId,c=currency},t));
   await c.ExecuteAsync(new CommandDefinition("insert into economy_ledger(player_id,currency,delta,reason,idempotency_key,balance_after) values(@p,@c,@d,@r,@k,@b)",new{p=playerId,c=currency,d=delta,r=reason,k=idempotencyKey,b=next},t));
   await audit.WriteAsync(c,t,auth.ActorId,playerId,"economy",reason,idempotencyKey,ct); return next;
 },ct);
}
public sealed class InventoryService(ITransactionRunner tx, AuditService audit)
{
 public Task TransferAsync(Guid from,Guid to,Guid itemInstanceId,long quantity,string idempotencyKey,IAuthorityContext auth,CancellationToken ct)=>tx.RunAsync(async(c,t)=>{
   if(quantity<=0)throw new ArgumentOutOfRangeException(nameof(quantity));
   var prior=await c.ExecuteScalarAsync<int>(new CommandDefinition("select count(*) from idempotency_keys where idempotency_key=@k",new{k=idempotencyKey},t));
   if(prior>0)return Task.CompletedTask;
   var item=await c.QuerySingleOrDefaultAsync<(Guid PlayerId,long Quantity)>(new CommandDefinition("select player_id as PlayerId,quantity as Quantity from inventory_items where item_instance_id=@id for update",new{id=itemInstanceId},t));
   if(item.PlayerId!=from||item.Quantity<quantity)throw new UnauthorizedAccessException("Item ownership or quantity check failed.");
   if(item.Quantity==quantity){await c.ExecuteAsync(new CommandDefinition("update inventory_items set player_id=@to,updated_at=now(),version=version+1 where item_instance_id=@id",new{to,id=itemInstanceId},t));}
   else {await c.ExecuteAsync(new CommandDefinition("update inventory_items set quantity=quantity-@q,version=version+1,updated_at=now() where item_instance_id=@id",new{q=quantity,id=itemInstanceId},t)); await c.ExecuteAsync(new CommandDefinition("insert into inventory_items(item_instance_id,player_id,item_id,quantity,durability,metadata_json) select gen_random_uuid(),@to,item_id,@q,durability,metadata_json from inventory_items where item_instance_id=@id",new{to,q=quantity,id=itemInstanceId},t));}
   await c.ExecuteAsync(new CommandDefinition("insert into idempotency_keys(idempotency_key,player_id,new_version,result_json) values(@k,@p,0,'{}'::jsonb)",new{k=idempotencyKey,p=from},t));
   await audit.WriteAsync(c,t,auth.ActorId,from,"inventory","transfer",idempotencyKey,ct); return Task.CompletedTask;
 },ct);
}
public sealed class SupremeRuinService(ITransactionRunner tx, AuditService audit)
{
 public Task ClaimInheritanceAsync(Guid ruinId,Guid playerId,IAuthorityContext auth,string idempotencyKey,CancellationToken ct)=>tx.RunAsync(async(c,t)=>{
   var prior=await c.ExecuteScalarAsync<int>(new CommandDefinition("select count(*) from idempotency_keys where idempotency_key=@k",new{k=idempotencyKey},t));
   if(prior>0)return Task.CompletedTask;
   var ruin=await c.QuerySingleAsync<(string GodId,string State,Guid? Challenger)>(new CommandDefinition("select god_id as GodId,state as State,challenger_player_id as Challenger from supreme_ruins where ruin_id=@r for update",new{r=ruinId},t));
   if(ruin.State!="InChallenge"||ruin.Challenger!=playerId)throw new InvalidOperationException("Player does not own the active Ruin challenge.");
   var kills=await c.ExecuteScalarAsync<int>(new CommandDefinition("select count(*) from ruin_divine_creatures where ruin_id=@r and killed=true",new{r=ruinId},t));
   if(kills!=5)throw new InvalidOperationException("All five Divine Creature kills are required.");
   var already=await c.ExecuteScalarAsync<int>(new CommandDefinition("select count(*) from supreme_ruins where god_id=@g and inheritance_owner_id is not null",new{g=ruin.GodId},t));
   if(already>0)throw new InvalidOperationException("This God inheritance has already been claimed.");
   await c.ExecuteAsync(new CommandDefinition("update supreme_ruins set state='Claimed',inheritance_owner_id=@p,completed_at=now(),updated_at=now(),version=version+1 where ruin_id=@r",new{p=playerId,r=ruinId},t));
   await c.ExecuteAsync(new CommandDefinition("insert into inheritance_history(ruin_id,god_id,player_id,action) values(@r,@g,@p,'CLAIM')",new{r=ruinId,g=ruin.GodId,p=playerId},t));
   await c.ExecuteAsync(new CommandDefinition("insert into idempotency_keys(idempotency_key,player_id,new_version,result_json) values(@k,@p,0,'{}'::jsonb)",new{k=idempotencyKey,p=playerId},t));
   await audit.WriteAsync(c,t,auth.ActorId,playerId,"ruin","inheritance.claim",idempotencyKey,ct); return Task.CompletedTask;
 },ct);
}
