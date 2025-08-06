FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["ProjectPlanner.CQRS/ProjectPlanner.Api/ProjectPlanner.Api.csproj", "ProjectPlanner.Api/"]
COPY ["ProjectPlanner.CQRS/ProjectPlanner.Commands/ProjectPlanner.Commands.csproj", "ProjectPlanner.Commands/"]
COPY ["ProjectPlanner.CQRS/ProjectPlanner.Queries/ProjectPlanner.Queries.csproj", "ProjectPlanner.Queries/"]
COPY ["ProjectPlanner.CQRS/ProjectPlanner.Domain/ProjectPlanner.Domain.csproj", "ProjectPlanner.Domain/"]

# Restore dependencies
RUN dotnet restore "ProjectPlanner.Api/ProjectPlanner.Api.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/ProjectPlanner.CQRS/ProjectPlanner.Api"
RUN dotnet build "ProjectPlanner.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ProjectPlanner.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectPlanner.Api.dll"]