FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build_webapp

WORKDIR /app

COPY Directory.Packages.props Directory.Packages.props
COPY Directory.Build.props Directory.Build.props

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api

COPY ScriptBeeWebApp/src/Common ScriptBeeWebApp/src/Common
COPY ScriptBeeWebApp/src/Workspace ScriptBeeWebApp/src/Workspace
COPY ScriptBeeWebApp/src/Analysis/Application ScriptBeeWebApp/src/Analysis/Application
COPY ScriptBeeWebApp/src/Analysis/Adapters ScriptBeeWebApp/src/Analysis/Adapters

RUN dotnet restore ScriptBeeWebApp/src/Analysis/Adapters/Analysis.Web/Analysis.Web.csproj

RUN dotnet publish ScriptBeeWebApp/src/Analysis/Adapters/Analysis.Web/Analysis.Web.csproj -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80

WORKDIR /app

COPY --from=build_webapp /app/publish .

ENTRYPOINT ["dotnet", "Analysis.Web.dll"]
