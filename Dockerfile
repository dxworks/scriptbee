FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ScriptBeeWebApp/ScriptBeeWebApp.csproj", "ScriptBeeWebApp/"]
COPY ["ScriptBee/ScriptBee.csproj", "ScriptBee/"]
COPY ["HelperFunctions/HelperFunctions.csproj", "HelperFunctions/"]
COPY ["DxWorks.ScriptBee.Plugin.Api/DxWorks.ScriptBee.Plugin.Api.csproj", "DxWorks.ScriptBee.Plugin.Api/"]
RUN dotnet restore "ScriptBeeWebApp/ScriptBeeWebApp.csproj"
COPY DxWorks.ScriptBee.Plugin.Api DxWorks.ScriptBee.Plugin.Api
COPY ScriptBeeWebApp ScriptBeeWebApp
COPY ScriptBee ScriptBee
COPY HelperFunctions HelperFunctions
WORKDIR "/src/ScriptBeeWebApp"
RUN dotnet build "ScriptBeeWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScriptBeeWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScriptBeeWebApp.dll"]
