# Use a imagem base do .NET 8 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use a imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie os arquivos de projeto
COPY ["FGC.Presentation/FGC.Presentation.csproj", "FGC.Presentation/"]
COPY ["FGC.Application/FGC.Application.csproj", "FGC.Application/"]
COPY ["FGC.Domain/FGC.Domain.csproj", "FGC.Domain/"]
COPY ["FGC.Infrastructure/FGC.Infrastructure.csproj", "FGC.Infrastructure/"]

# Restaure as dependências
RUN dotnet restore "FGC.Presentation/FGC.Presentation.csproj"

# Copie todo o código
COPY . .

# Build da aplicação
WORKDIR "/src/FGC.Presentation"
RUN dotnet build "FGC.Presentation.csproj" -c Release -o /app/build

# Publique a aplicação
FROM build AS publish
RUN dotnet publish "FGC.Presentation.csproj" -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FGC.Presentation.dll"]