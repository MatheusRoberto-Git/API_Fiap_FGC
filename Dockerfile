# Use a imagem base do .NET 8 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use a imagem SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["FGC.Presentation/FGC.Presentation.csproj", "FGC.Presentation/"]
COPY ["FGC.Application/FGC.Application.csproj", "FGC.Application/"]
COPY ["FGC.Infrastructure/FGC.Infrastructure.csproj", "FGC.Infrastructure/"]
COPY ["FGC.Domain/FGC.Domain.csproj", "FGC.Domain/"]

RUN dotnet restore "./FGC.Presentation/FGC.Presentation.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/FGC.Presentation"
RUN dotnet build "./FGC.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar a aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FGC.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Variáveis de ambiente para produção
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Healthcheck
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FGC.Presentation.dll"]