﻿FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build_webapp

WORKDIR /app

COPY Directory.Packages.props Directory.Packages.props
COPY Directory.Build.props Directory.Build.props

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api

COPY src/Common src/Common

COPY src/Application/Domain src/Application/Domain

COPY src/Application/Ports/Driving src/Application/Ports/Driving
COPY src/Application/Ports/Driven/Ports.Analysis src/Application/Ports/Driven/Ports.Analysis
COPY src/Application/Ports/Driven/Ports.Project src/Application/Ports/Driven/Ports.Project

COPY src/Adapters/Driving/Common.Web src/Adapters/Driving/Common.Web
COPY src/Adapters/Driving/Calculation.Web src/Adapters/Driving/Calculation.Web

RUN dotnet restore src/Adapters/Driving/Calculation.Web

RUN dotnet publish src/Adapters/Driving/Calculation.Web -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY --from=build_webapp /app/publish .

ENTRYPOINT ["dotnet", "Calculation.Web.dll"]
