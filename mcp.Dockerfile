FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build_webapp

WORKDIR /app

COPY Directory.Packages.props Directory.Packages.props
COPY Directory.Build.props Directory.Build.props

COPY docs/public/gateway_swagger.json docs/public/gateway_swagger.json

COPY Integrations/MCP/ScriptBee.MCP Integrations/MCP/ScriptBee.MCP

RUN dotnet restore Integrations/MCP/ScriptBee.MCP/ScriptBee.MCP.csproj

RUN dotnet publish Integrations/MCP/ScriptBee.MCP/ScriptBee.MCP.csproj -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80

WORKDIR /app

COPY --from=build_webapp /app/publish .

ENTRYPOINT ["dotnet", "ScriptBee.MCP.dll"]
