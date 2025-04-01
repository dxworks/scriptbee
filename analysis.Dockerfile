FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build_webapp

WORKDIR /app

COPY Directory.Packages.props Directory.Packages.props
COPY Directory.Build.props Directory.Build.props

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api

COPY src/Common src/Common

COPY src/Application/Domain/Model src/Application/Domain/Model
COPY src/Application/Domain/Service.Analysis src/Application/Domain/Service.Analysis
COPY src/Application/Domain/Service.Plugin src/Application/Domain/Service.Plugin

COPY src/Application/Ports/Driving/UseCases.Analysis src/Application/Ports/Driving/UseCases.Analysis
COPY src/Application/Ports/Driving/UseCases.Plugin src/Application/Ports/Driving/UseCases.Plugin

COPY src/Application/Ports/Driven/Ports.Analysis src/Application/Ports/Driven/Ports.Analysis
COPY src/Application/Ports/Driven/Ports.Files src/Application/Ports/Driven/Ports.Files
COPY src/Application/Ports/Driven/Ports.Instance src/Application/Ports/Driven/Ports.Instance
COPY src/Application/Ports/Driven/Ports.Plugins src/Application/Ports/Driven/Ports.Plugins
COPY src/Application/Ports/Driven/Ports.Project src/Application/Ports/Driven/Ports.Project

COPY src/Adapters/Driven/Persistence.File src/Adapters/Driven/Persistence.File
COPY src/Adapters/Driven/Persistence.InMemory src/Adapters/Driven/Persistence.InMemory
COPY src/Adapters/Driven/Persistence.Mongodb src/Adapters/Driven/Persistence.Mongodb

COPY src/Adapters/Driving/Common.Web src/Adapters/Driving/Common.Web
COPY src/Adapters/Driving/Analysis.Web src/Adapters/Driving/Analysis.Web

RUN dotnet restore src/Adapters/Driving/Analysis.Web

RUN dotnet publish src/Adapters/Driving/Analysis.Web -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY --from=build_webapp /app/publish .

ENTRYPOINT ["dotnet", "Analysis.Web.dll"]
