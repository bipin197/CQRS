FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# First copy all project files
COPY ["./ProjectPlanner.Api/ProjectPlanner.Api.csproj", "ProjectPlanner.Api/"]
COPY ["./ProjectPlanner.Commands/ProjectPlanner.Commands.csproj", "ProjectPlanner.Commands/"]
COPY ["./ProjectPlanner.Queries/ProjectPlanner.Queries.csproj", "ProjectPlanner.Queries/"]
COPY ["./ProjectPlanner.Domain/ProjectPlanner.Domain.csproj", "ProjectPlanner.Domain/"]

# Restore NuGet packages
RUN dotnet restore "ProjectPlanner.Api/ProjectPlanner.Api.csproj"

# Copy the source code
COPY ["./ProjectPlanner.Api/", "ProjectPlanner.Api/"]
COPY ["./ProjectPlanner.Commands/", "ProjectPlanner.Commands/"]
COPY ["./ProjectPlanner.Queries/", "ProjectPlanner.Queries/"]
COPY ["./ProjectPlanner.Domain/", "ProjectPlanner.Domain/"]

WORKDIR "/src/ProjectPlanner.Api"
RUN dotnet build "ProjectPlanner.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectPlanner.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectPlanner.Api.dll"]