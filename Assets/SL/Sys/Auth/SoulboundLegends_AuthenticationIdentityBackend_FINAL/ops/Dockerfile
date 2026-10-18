FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/SoulboundLegends.Auth.Api/SoulboundLegends.Auth.Api.csproj
RUN dotnet publish src/SoulboundLegends.Auth.Api/SoulboundLegends.Auth.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "SoulboundLegends.Auth.Api.dll"]
