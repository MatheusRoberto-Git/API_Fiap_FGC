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

# Restaure as depend�ncias
RUN dotnet restore "FGC.Presentation/FGC.Presentation.csproj"

# Copie todo o c�digo
COPY . .

# Build da aplica��o
WORKDIR "/src/FGC.Presentation"
RUN dotnet build "FGC.Presentation.csproj" -c Release -o /app/build

# Publique a aplica��o
FROM build AS publish
RUN dotnet publish "FGC.Presentation.csproj" -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FGC.Presentation.dll"]