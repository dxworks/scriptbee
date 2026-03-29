# Build the client app
FROM node:24 AS build_client

WORKDIR /src

COPY ./ScriptBeeClient/package*.json ./

RUN npm install

COPY ./ScriptBeeClient/src ./src
COPY ./ScriptBeeClient/angular.json .
COPY ./ScriptBeeClient/tsconfig*.json ./

RUN npm run build-prod

# Build the backend web app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build_webapp

WORKDIR /app

COPY Directory.Packages.props Directory.Packages.props
COPY Directory.Build.props Directory.Build.props

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api

COPY ScriptBeeWebApp/src/Common ScriptBeeWebApp/src/Common
COPY ScriptBeeWebApp/src/Workspace ScriptBeeWebApp/src/Workspace
COPY ScriptBeeWebApp/src/Gateway/Application ScriptBeeWebApp/src/Gateway/Application
COPY ScriptBeeWebApp/src/Gateway/Adapters ScriptBeeWebApp/src/Gateway/Adapters

RUN dotnet restore ScriptBeeWebApp/src/Gateway/Adapters/Web/Web.csproj

RUN dotnet publish ScriptBeeWebApp/src/Gateway/Adapters/Web/Web.csproj -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY --from=build_webapp /app/publish .

COPY --from=build_client /src/dist/script-bee-ui /app/wwwroot

#ENV LD_LIBRARY_PATH=/app/runtimes/debian.9-x64/native/

ENTRYPOINT ["dotnet", "Web.dll"]
