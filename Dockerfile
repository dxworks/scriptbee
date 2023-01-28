# Build the client app
FROM node:18 as build_client

WORKDIR /src

COPY ./ScriptBeeClient/package*.json ./

RUN npm install

COPY ./ScriptBeeClient/src ./src
COPY ./ScriptBeeClient/angular.json .
COPY ./ScriptBeeClient/tsconfig*.json ./
COPY ./ScriptBeeClient/webpack*.config.js ./

RUN npm run build-prod

# Build the backend web app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build_webapp

WORKDIR /app

COPY DxWorks.ScriptBee.Plugin.Api ./DxWorks.ScriptBee.Plugin.Api
COPY ScriptBee ./ScriptBee
COPY ScriptBee.Marketplace.Client ./ScriptBee.Marketplace.Client
COPY ScriptBeeWebApp ./ScriptBeeWebApp

RUN dotnet restore ./ScriptBeeWebApp

RUN dotnet publish ./ScriptBeeWebApp -c Release -o publish --no-restore

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY --from=build_webapp /app/publish .

COPY --from=build_client /src/dist/scriptbeewebapp /app/wwwroot

ENV LD_LIBRARY_PATH=/app/runtimes/debian.9-x64/native/

ENTRYPOINT ["dotnet", "ScriptBeeWebApp.dll"]
