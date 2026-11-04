FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/CleanArchitecture.Api/CleanArchitecture.Api.csproj", "src/API/CleanArchitecture.Api/"]
COPY ["src/Core/CleanArchitecture.Application/CleanArchitecture.Application.csproj", "src/Core/CleanArchitecture.Application/"]
COPY ["src/Core/CleanArchitecture.Domain/CleanArchitecture.Domain.csproj", "src/Core/CleanArchitecture.Domain/"]
COPY ["src/Infrastructure/CleanArchitecture.Identity/CleanArchitecture.Identity.csproj", "src/Infrastructure/CleanArchitecture.Identity/"]
COPY ["src/Infrastructure/CleanArchitecture.Infrastructure/CleanArchitecture.Infrastructure.csproj", "src/Infrastructure/CleanArchitecture.Infrastructure/"]
COPY ["src/Infrastructure/CleanArchitecture.Persistence/CleanArchitecture.Persistence.csproj", "src/Infrastructure/CleanArchitecture.Persistence/"]
RUN dotnet restore "src/API/CleanArchitecture.Api/CleanArchitecture.Api.csproj"
COPY . .
WORKDIR "/src/src/API/CleanArchitecture.Api"
RUN dotnet build "CleanArchitecture.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CleanArchitecture.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]
