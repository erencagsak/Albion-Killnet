FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/AlbionKillnet.Api/AlbionKillnet.Api.csproj", "src/AlbionKillnet.Api/"]
COPY ["src/AlbionKillnet.Core/AlbionKillnet.Core.csproj", "src/AlbionKillnet.Core/"]

RUN dotnet restore "src/AlbionKillnet.Api/AlbionKillnet.Api.csproj"

COPY . .

WORKDIR "/src/src/AlbionKillnet.Api"
RUN dotnet build "AlbionKillnet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlbionKillnet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlbionKillnet.Api.dll"]