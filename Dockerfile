# =================================================================
# FIAP Cloud Games (FCG) - Dockerfile
# Multi-stage build
# =================================================================

# ===== STAGE 1: BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas dos arquivos de projeto primeiro (cache do Docker)
COPY ["FGC.Presentation/FGC.Presentation.csproj", "FGC.Presentation/"]
COPY ["FGC.Application/FGC.Application.csproj", "FGC.Application/"]
COPY ["FGC.Domain/FGC.Domain.csproj", "FGC.Domain/"]
COPY ["FGC.Infrastructure/FGC.Infrastructure.csproj", "FGC.Infrastructure/"]

# Restaurar dependências (aproveita cache se não mudou)
RUN dotnet restore "FGC.Presentation/FGC.Presentation.csproj"

# Copia todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/FGC.Presentation"
RUN dotnet build "FGC.Presentation.csproj" -c Release -o /app/build

# ===== STAGE 2: PUBLISH =====
FROM build AS publish
RUN dotnet publish "FGC.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== STAGE 3: RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Configurar usuário não-root para segurança
RUN addgroup --system --gid 1001 fcggroup
RUN adduser --system --uid 1001 fcguser

# Instalar dependências necessárias
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copiar os arquivos publicados
COPY --from=publish /app/publish .

# Configurar permissões
RUN chown -R fcguser:fcggroup /app
USER fcguser

# Configuração das variáveis de ambiente
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Expor a porta
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Comando de inicialização
ENTRYPOINT ["dotnet", "FGC.Presentation.dll"]