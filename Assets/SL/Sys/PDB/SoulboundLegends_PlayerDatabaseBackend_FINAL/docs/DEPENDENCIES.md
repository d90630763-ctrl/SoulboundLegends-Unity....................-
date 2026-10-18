# Dependencies

Backend NuGet dependencies are declared in the project files: Dapper, Npgsql, ASP.NET Core JWT bearer authentication, OpenAPI, and xUnit/FluentAssertions for tests.

Runtime external services: PostgreSQL, trusted identity provider/JWT issuer, secret management, TLS termination, backup storage, monitoring/alerting. These are intentionally not bundled because embedding credentials or pretending to provide cloud redundancy would be unsafe.

Unity requires a Unity project compatible with the C# API surface used by the included runtime package. The client uses `UnityEngine` and standard .NET HTTP/JSON APIs.
