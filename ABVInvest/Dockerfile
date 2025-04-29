FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ABVInvest/ABVInvest.csproj", "ABVInvest/"]
COPY ["ABVInvest.Client/ABVInvest.Client.csproj", "ABVInvest.Client/"]
COPY ["ABVInvest.Common/ABVInvest.Common.csproj", "ABVInvest.Common/"]
COPY ["ABVInvest.Data/ABVInvest.Data.csproj", "ABVInvest.Data/"]
COPY ["ABVInvest.Services/ABVInvest.Services.csproj", "ABVInvest.Services/"]
RUN dotnet restore "./ABVInvest/ABVInvest.csproj"
COPY . .
WORKDIR "/src/ABVInvest"
RUN dotnet build "./ABVInvest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ABVInvest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ABVInvest.dll"]