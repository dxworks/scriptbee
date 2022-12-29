FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api
COPY ScriptBee ./ScriptBee
COPY ScriptBee.Marketplace.Client ./ScriptBee.Marketplace.Client
COPY ScriptBeeWebApp ./ScriptBeeWebApp

RUN dotnet restore ./ScriptBeeWebApp

RUN dotnet publish ./ScriptBeeWebApp -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0  AS final

EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY --from=build /app/publish .

ENV LD_LIBRARY_PATH=/app/runtimes/debian.9-x64/native/

ENTRYPOINT ["dotnet", "ScriptBeeWebApp.dll"]
