FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore (layer-cache friendly)
COPY ["src/CleanInk.OnCall.Api/CleanInk.OnCall.Api.csproj", "src/CleanInk.OnCall.Api/"]
COPY ["src/CleanInk.OnCall.Application/CleanInk.OnCall.Application.csproj", "src/CleanInk.OnCall.Application/"]
COPY ["src/CleanInk.OnCall.Domain/CleanInk.OnCall.Domain.csproj", "src/CleanInk.OnCall.Domain/"]
COPY ["src/CleanInk.OnCall.Infrastructure/CleanInk.OnCall.Infrastructure.csproj", "src/CleanInk.OnCall.Infrastructure/"]
COPY ["src/CleanInk.OnCall.Shared/CleanInk.OnCall.Shared.csproj", "src/CleanInk.OnCall.Shared/"]
RUN dotnet restore "src/CleanInk.OnCall.Api/CleanInk.OnCall.Api.csproj"

# Copy source and build
COPY . .
RUN dotnet build "src/CleanInk.OnCall.Api/CleanInk.OnCall.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/CleanInk.OnCall.Api/CleanInk.OnCall.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CleanInk.OnCall.Api.dll"]
