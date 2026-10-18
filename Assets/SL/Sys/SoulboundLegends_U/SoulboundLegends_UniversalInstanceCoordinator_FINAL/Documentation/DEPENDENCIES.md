# Dependencies

## Unity
- Unity 6.0.3 or later in the Unity 6 line.
- Unity Test Framework for the included Editor tests.
- No mandatory third-party Unity package dependency.

## Server
- .NET 8 SDK/runtime.
- ASP.NET Core runtime supplied by the .NET 8 shared framework.
- No third-party NuGet dependency.
- A reachable authoritative instance runtime agent implementing the documented HTTP provisioning contract.

## Project integrations
The coordinator intentionally integrates through interfaces. The combined SL project supplies the concrete Network Transport, Master Authority/Server Authorization, Anti-Cheat, Database/Persistence, AOI, Matchmaking and gameplay systems.
