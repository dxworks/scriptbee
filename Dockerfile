# Build the client app
FROM node:22 AS build_client

WORKDIR /src

COPY ./ScriptBeeClient/package*.json ./

RUN npm install

COPY ./ScriptBeeClient/src ./src
COPY ./ScriptBeeClient/angular.json .
COPY ./ScriptBeeClient/tsconfig*.json ./

RUN npm run build-prod

# Build the backend web app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build_webapp

WORKDIR /app

COPY Directory.Packages.props Directory.Packages.props
COPY Directory.Build.props Directory.Build.props

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api
# COPY ScriptBee.Marketplace.Client ./ScriptBee.Marketplace.Client
COPY src/Common src/Common

COPY src/Application/Domain/Model src/Application/Domain/Model
COPY src/Application/Domain/Service.Project src/Application/Domain/Service.Project

COPY src/Application/Ports/Driving/UseCases.Project src/Application/Ports/Driving/UseCases.Project
COPY src/Application/Ports/Driven/Ports.Analysis src/Application/Ports/Driven/Ports.Analysis
COPY src/Application/Ports/Driven/Ports.Instance src/Application/Ports/Driven/Ports.Instance
COPY src/Application/Ports/Driven/Ports.Project src/Application/Ports/Driven/Ports.Project

COPY src/Adapters/Driven/Persistence.Mongodb src/Adapters/Driven/Persistence.Mongodb
COPY src/Adapters/Driven/Analysis.Instance.Docker src/Adapters/Driven/Analysis.Instance.Docker

COPY src/Adapters/Driving/Common.Web src/Adapters/Driving/Common.Web
COPY src/Adapters/Driving/Web src/Adapters/Driving/Web

RUN dotnet restore ./src/Adapters/Driving/Web

RUN dotnet publish ./src/Adapters/Driving/Web -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY --from=build_webapp /app/publish .

COPY --from=build_client /src/dist/script-bee-ui /app/wwwroot

#ENV LD_LIBRARY_PATH=/app/runtimes/debian.9-x64/native/

ENTRYPOINT ["dotnet", "Web.dll"]
