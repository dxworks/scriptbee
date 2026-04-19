# Build the client app
FROM node:24 AS build_client

WORKDIR /src

COPY ScriptBeeClient/package*.json ./
RUN npm ci

COPY ScriptBeeClient/src ./src
COPY ScriptBeeClient/public ./public
COPY ScriptBeeClient/angular.json .
COPY ScriptBeeClient/tsconfig.json .
COPY ScriptBeeClient/tsconfig.app.json .

RUN npm run build-prod

# Build the backend web app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build_webapp

WORKDIR /app

COPY Directory.Packages.props Directory.Packages.props
COPY Directory.Build.props Directory.Build.props

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api

COPY ScriptBeeWebApp/src/Common ScriptBeeWebApp/src/Common
COPY ScriptBeeWebApp/src/Workspace ScriptBeeWebApp/src/Workspace
COPY ScriptBeeWebApp/src/Plugins ScriptBeeWebApp/src/Plugins
COPY ScriptBeeWebApp/src/Gateway/Application ScriptBeeWebApp/src/Gateway/Application
COPY ScriptBeeWebApp/src/Gateway/Adapters ScriptBeeWebApp/src/Gateway/Adapters

COPY Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp
COPY Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript
COPY Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Python Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Python

RUN dotnet restore ScriptBeeWebApp/src/Gateway/Adapters/Web/Web.csproj

RUN dotnet publish ScriptBeeWebApp/src/Gateway/Adapters/Web/Web.csproj -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80

WORKDIR /app

COPY --from=build_webapp /app/publish .

COPY --from=build_client /src/dist/script-bee-ui/browser ./wwwroot

ENTRYPOINT ["dotnet", "Web.dll"]
