# 🎮 FIAP Cloud Games (FCG) - API

> **Tech Challenge - Fase 1**  
> API REST em .NET 8 para gerenciamento de usuários e biblioteca de jogos, aplicando Domain-Driven Design (DDD) e Clean Architecture.

> **Tech Challenge - Fase 2**  
> Evolução da API com CI/CD, Dockerização, Cloud Deployment e Monitoramento para garantir escalabilidade e resiliência.

---

## 📋 Índice

- [🎯 Objetivos](#-objetivos)
- [🏗️ Arquitetura](#️-arquitetura)
- [🚀 Como Executar](#-como-executar)
- [🔄 CI/CD e Deployment](#-cicd-e-deployment)
- [📊 Monitoramento](#-monitoramento)
- [📚 Endpoints da API](#-endpoints-da-api)
- [🔐 Autenticação JWT](#-autenticação-jwt)
- [🧪 Testes TDD](#-testes-tdd)
- [📖 Event Storming](#-event-storming)
- [🛠️ Tecnologias](#️-tecnologias)
- [👥 Equipe](#-equipe)

---

## 🎯 Objetivos

### Objetivo Geral
Desenvolver a **plataforma FIAP Cloud Games** com deploy automatizado, containerização e monitoramento em cloud, garantindo escalabilidade, resiliência e observabilidade.

### Objetivos Específicos - Fase 1
- ✅ **Implementar Clean Architecture** com DDD
- ✅ **Criar API REST** em .NET 8 com documentação Swagger
- ✅ **Garantir autenticação segura** via JWT
- ✅ **Aplicar TDD** em pelo menos um módulo
- ✅ **Separar responsabilidades** entre usuários e administradores
- ✅ **Documentar arquitetura** com Event Storming

### Objetivos Específicos - Fase 2
- ✅ **Dockerizar a aplicação** com multi-stage build otimizado
- ✅ **Implementar CI/CD** com GitHub Actions (pipelines automatizados)
- ✅ **Deploy na Cloud** Azure Container Instances
- ✅ **Configurar monitoramento** Azure Monitor + Datadog
- ✅ **Garantir escalabilidade** e alta disponibilidade
- ✅ **Automatizar testes** e deployment contínuo

---

## 🏗️ Arquitetura

### Clean Architecture + DDD

```
┌─────────────────────────────────────┐
│           Presentation              │ ← Controllers, Models, JWT
├─────────────────────────────────────┤
│           Application               │ ← Use Cases, DTOs
├─────────────────────────────────────┤
│             Domain                  │ ← Entities, Value Objects, Events
├─────────────────────────────────────┤
│          Infrastructure             │ ← Repositories, DbContext, Services
└─────────────────────────────────────┘
```

### Contextos Delimitados (Bounded Contexts)
- **👤 User Management**: Cadastro, autenticação e gestão de usuários
- **🎮 Game Catalog**: Catálogo de jogos (futuro)
- **📚 Game Library**: Biblioteca de jogos do usuário (futuro)
- **🎉 Promotion**: Sistema de promoções (futuro)

### Estrutura de Projetos
```
API_Fiap_FGC/
├── .github/workflows/       # CI/CD Pipelines
│   ├── ci.yml              # Continuous Integration
│   └── cd.yml              # Continuous Deployment
├── FGC.Domain/             # Regras de negócio e entidades
├── FGC.Application/        # Casos de uso e orquestração
├── FGC.Infrastructure/     # Acesso a dados e serviços externos
├── FGC.Presentation/       # Controllers e modelos de API
├── FGC.Domain.Tests/       # Testes unitários com TDD
└── Dockerfile              # Imagem Docker multi-stage
```

---

## 🚀 Como Executar

### Pré-requisitos
- **.NET 8 SDK** ou superior
- **Docker Desktop** (para containerização)
- **Azure CLI** (para deploy na cloud)
- **IDE**: Visual Studio 2022 ou VS Code
- **Git** para clonagem do repositório

### Executar Localmente

```bash
# 1. Clonar repositório
git clone https://github.com/MatheusRoberto-Git/API_Fiap_FGC.git
cd API_Fiap_FGC

# 2. Restaurar dependências
dotnet restore

# 3. Executar aplicação
dotnet run --project FGC.Presentation

# 4. Acessar
# Swagger: https://localhost:61043
# Health: https://localhost:61043/health
```

### Executar com Docker

```bash
# Build da imagem
docker build -t fgc-api:local .

# Executar container
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="SuaConnectionString" \
  -e Jwt__SecretKey="SuaChaveSecreta" \
  --name fgc-api-local \
  fgc-api:local

# Testar
curl http://localhost:8080/health
```

### Acessar em Produção

A aplicação está publicada na Azure e pode ser acessada em:

- **API Base**: `http://fgc-api-v1.eastus2.azurecontainer.io:8080/`
- **Health Check**: `http://fgc-api-v1.eastus2.azurecontainer.io:8080/health`
- **Swagger**: `http://fgc-api-v1.eastus2.azurecontainer.io:8080/index.html`

---

## 🔄 CI/CD e Deployment

### Pipeline CI (Continuous Integration)

**Quando executa:** Pull Request ou push para `develop`

**O que faz:**
- ✅ Restaura dependências
- ✅ Build da solução
- ✅ Executa testes unitários
- ✅ Valida qualidade do código
- ✅ Build de teste da imagem Docker

**Arquivo:** `.github/workflows/ci.yml`

### Pipeline CD (Continuous Deployment)

**Quando executa:** Push/merge para `master`

**O que faz:**
- ✅ Build da aplicação
- ✅ Executa testes
- ✅ Build da imagem Docker
- ✅ Push para Azure Container Registry
- ✅ Deploy no Azure Container Instance
- ✅ Health check automático
- ✅ Notificação de deployment no Datadog

**Arquivo:** `.github/workflows/cd.yml`

### Infraestrutura Azure

**Recursos provisionados:**
- **Azure Container Registry** (`fgcregistry`) - Armazenamento de imagens Docker
- **Azure Container Instance** (`fgc-api-container`) - Hosting da aplicação
- **Azure SQL Database** (`fgc-database`) - Banco de dados
- **SQL Server** (`fgc-sql-server`) - Servidor de banco
- **Resource Group** (`rg-fgc-api`) - Agrupamento de recursos

### Docker

**Dockerfile multi-stage** otimizado para:
- ✅ Imagem final enxuta (~200MB)
- ✅ Separação de build e runtime
- ✅ Cache de layers eficiente
- ✅ Datadog APM integrado
- ✅ Health check configurado

---

## 📊 Monitoramento

A aplicação possui stack completa de monitoramento para garantir observabilidade e detecção proativa de problemas.

### Azure Monitor (Logs e Métricas)

**Acesso via Portal Azure:**
```
Container Instances > fgc-api-container > Logs/Metrics
```

**O que monitora:**
- 📝 Logs de aplicação em tempo real
- 📊 CPU Usage
- 💾 Memory Working Set
- 🌐 Network In/Out
- ⚠️ Errors e Warnings

**Via CLI:**
```bash
# Logs em tempo real
az container logs --resource-group rg-fgc-api \
  --name fgc-api-container --follow

# Métricas
az monitor metrics list --resource fgc-api-container
```

### Datadog (Events e APM)

**Dashboard:** `https://app.datadoghq.com`

**O que rastreia:**
- 🚀 Eventos de deployment
- ✅ Status de CI/CD
- 📦 Versões deployadas
- ⏱️ Tempo de deployment

**Configuração:**
- Service: `fgc-api`
- Environment: `production`
- Site: `datadoghq.com`

### Health Checks

**Endpoint:**
```bash
curl http://fgc-api-v1.eastus2.azurecontainer.io:8080/health
```

**Docker Healthcheck:**
- Intervalo: 30s
- Timeout: 10s
- Retries: 3

---

## 📚 Endpoints da API

### 🔓 Endpoints Públicos

#### Autenticação
```http
POST /api/auth/login          # Fazer login e obter token JWT
POST /api/auth/logout         # Fazer logout
POST /api/auth/validate-token # Validar token JWT
```

#### Usuários
```http
POST /api/users/register      # Registrar novo usuário
GET  /api/users/profile/{id}  # Obter perfil do usuário
PUT  /api/users/changePassword/{id} # Alterar senha
```

### 🔐 Endpoints Protegidos (JWT Required)

#### Administração (Role: Admin)
```http
POST /api/admin/create           # Criar novo administrador
PUT  /api/admin/promote          # Promover usuário a admin
PUT  /api/admin/demote/{id}      # Despromover admin
PUT  /api/admin/deactivate/{id}  # Desativar usuário
PUT  /api/admin/reactivate/{id}  # Reativar usuário
GET  /api/admin/adminLogged      # Informações do admin logado
```

---

## 🔐 Autenticação JWT

### Como Autenticar

#### 1. Registrar Usuário
```bash
curl -X POST http://fgc-api-v1.h2f6dpcqhbdzc5df.eastus2.azurecontainer.io:8080/api/users/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@fgc.com",
    "password": "MinhaSenh@123",
    "name": "Nome do Usuário"
  }'
```

#### 2. Fazer Login
```bash
curl -X POST http://fgc-api-v1.h2f6dpcqhbdzc5df.eastus2.azurecontainer.io:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@fgc.com",
    "password": "MinhaSenh@123"
  }'
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "guid",
      "email": "usuario@fgc.com",
      "name": "Nome do Usuário",
      "role": "User"
    },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2025-09-30T12:00:00Z",
    "tokenType": "Bearer"
  }
}
```

#### 3. Usar Token em Requests
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Configuração no Swagger
1. Clique no botão **"Authorize"** 🔒
2. Digite: `Bearer {seu_token_jwt}`
3. Clique em **"Authorize"**
4. Agora pode acessar endpoints protegidos

### Níveis de Acesso
- **🟢 Público**: Registro, login, visualizar perfis
- **🔵 Usuário**: Alterar própria senha, acessar biblioteca (futuro)
- **🔴 Admin**: Gerenciar usuários, criar admins, acessar relatórios

---

## 🧪 Testes TDD

### Metodologia Aplicada
Implementação de **Test-Driven Development** seguindo o ciclo **RED-GREEN-REFACTOR**.

#### Ciclo TDD
1. **🔴 RED**: Escrever teste que falha
2. **🟢 GREEN**: Implementar código mínimo para passar
3. **🔵 REFACTOR**: Melhorar código mantendo testes passando

### Módulo Testado: Value Object Email

#### Por que escolhi Email?
- ✅ **Regras claras**: Validação de formato, tamanho, normalização
- ✅ **Lógica isolada**: Sem dependências externas
- ✅ **Casos bem definidos**: Sucesso, falha e comportamentos
- ✅ **Ideal para TDD**: Demonstra conceitos fundamentais

#### Cobertura de Testes

##### 🟢 Cenários de Sucesso (6 testes)
| Teste | Validação |
|-------|-----------|
| `Constructor_WithValidSimpleEmail` | Email básico válido |
| `Constructor_WithValidEmailWithSubdomain` | Subdomínios |
| `Constructor_WithValidEmailWithNumbers` | Números no email |
| `Constructor_WithUppercaseEmail` | Normalização maiúscula→minúscula |
| `Constructor_WithEmailWithSpaces` | Remoção automática de espaços |
| `Constructor_WithValidLongEmail` | Emails longos até 254 caracteres |

##### 🔴 Cenários de Falha (9 testes)
| Teste | Validação de Exceção |
|-------|---------------------|
| `Constructor_WithNullEmail` | Email null → `ArgumentException` |
| `Constructor_WithEmptyEmail` | String vazia → `ArgumentException` |
| `Constructor_WithWhitespaceEmail` | Apenas espaços → `ArgumentException` |
| `Constructor_WithEmailWithoutAtSymbol` | Sem @ → `ArgumentException` |
| `Constructor_WithEmailWithoutDomain` | Sem domínio → `ArgumentException` |
| `Constructor_WithEmailWithoutLocalPart` | Sem parte local → `ArgumentException` |
| `Constructor_WithTooLongEmail` | Muito longo (+254) → `ArgumentException` |
| `Constructor_WithMultipleAtSymbols` | Múltiplos @ → `ArgumentException` |
| `Constructor_WithEmailWithSpacesInside` | Espaços internos → `ArgumentException` |

##### 🔄 Cenários de Comportamento (4 testes)
| Teste | Validação de Value Object |
|-------|---------------------------|
| `TwoEmailsWithSameValue_ShouldBeEqual` | Igualdade por valor |
| `TwoEmailsWithDifferentValues_ShouldNotBeEqual` | Desigualdade |
| `ToString_ShouldReturnEmailValue` | Método ToString() |
| `ImplicitConversion_ShouldReturnEmailValue` | Conversão implícita |

#### Executar Testes

```bash
# Todos os testes
dotnet test

# Por categoria
dotnet test --filter "Scenario=Success"
dotnet test --filter "Scenario=Failure"
dotnet test --filter "Scenario=Behavior"

# Com detalhes
dotnet test --verbosity detailed
```

#### Métricas
- **📊 Total**: 19 testes
- **✅ Sucesso**: 100% (19/19)
- **📈 Cobertura**: ~95% do Value Object Email

#### Ferramentas
- **xUnit**: Framework de testes
- **FluentAssertions**: Assertions expressivas

---

## 📖 Event Storming

### Fluxos Mapeados

#### 👤 Fluxo de Cadastro de Usuários
```
[Comando] Cadastra Usuário
    ↓
[Política] Check: Email existente
    ↓
[Política] Validar: Dados do usuário
    ↓
[Política] Save: SQL Server
    ↓
[Evento] Usuário Cadastrado
    ↓
[Evento] E-mail Confirmação
    ↓
[Política] Disparo NET Mail
```

#### 🎮 Fluxo de Cadastro de Jogos
```
[Comando] Cadastra Jogo (ADM)
    ↓
[Política] Check: Permissão ADM
    ↓
[Política] Validar: Dados do jogo
    ↓
[Política] Save: SQL Server
    ↓
[Evento] Jogo Cadastrado
    ↓
[Modelo Leitura] Catálogo de jogos
```

### Contextos Identificados
- **User Management**: Usuários, autenticação, perfis
- **Game Catalog**: Jogos, preços, categorias
- **Game Library**: Biblioteca pessoal, downloads
- **Promotion**: Descontos, ofertas especiais

---

## 🛠️ Tecnologias

### Backend
- **.NET 8**: Framework principal
- **ASP.NET Core**: Web API
- **Entity Framework Core**: ORM
- **JWT Bearer**: Autenticação
- **Swagger/OpenAPI**: Documentação

### Cloud & DevOps
- **Azure Container Registry**: Registry Docker
- **Azure Container Instances**: Hosting
- **Azure SQL Database**: Banco de dados
- **GitHub Actions**: CI/CD
- **Docker**: Containerização

### Monitoramento
- **Azure Monitor**: Logs e métricas nativas
- **Datadog**: APM e eventos de deployment
- **Container Insights**: Telemetria de containers

### Banco de Dados
- **SQL Server**: Produção
- **In-Memory Database**: Desenvolvimento e testes

### Testes
- **xUnit**: Framework de testes unitários
- **FluentAssertions**: Assertions expressivas

### Arquitetura
- **Clean Architecture**: Separação de responsabilidades
- **Domain-Driven Design**: Modelagem do domínio
- **CQRS**: Command Query Responsibility Segregation
- **Event Sourcing**: Eventos de domínio

---

## 👥 Equipe

### Desenvolvedor Principal
- **Nome**: Matheus Roberto de Oliveira
- **Discord**: .meister.m
- **GitHub**: MatheusRoberto-Git

### Papéis Técnicos
- **Solution Architect**: Desenho da arquitetura DDD e Clean Architecture
- **Backend Developer**: Implementação da API REST
- **DevOps Engineer**: CI/CD e infraestrutura na cloud
- **QA Engineer**: Implementação de testes TDD

---

## 📞 Suporte

### Links Úteis - Produção
- **API Base**: `http://fgc-api-v1.eastus2.azurecontainer.io:8080/`
- **Health Check**: `http://fgc-api-v1.eastus2.azurecontainer.io:8080/health`
- **Swagger**: `http://fgc-api-v1.eastus2.azurecontainer.io:8080/index.html`

### Links Úteis - Desenvolvimento
- **Repositório**: `https://github.com/MatheusRoberto-Git/API_Fiap_FGC`
- **CI/CD Pipelines**: `https://github.com/MatheusRoberto-Git/API_Fiap_FGC/actions`
- **Docker Registry**: Azure Container Registry (fgcregistry)

### Contato
- **Discord**: .meister.m
- **Email**: matheus.pro2@hotmail.com

---

## 📄 Licença

Este projeto foi desenvolvido como parte do **Tech Challenge - FIAP** e é destinado exclusivamente para fins educacionais.

---

## 🎯 Requisitos Atendidos

### Fase 1
- ✅ Clean Architecture + DDD
- ✅ API REST completa
- ✅ Autenticação JWT
- ✅ Testes TDD
- ✅ Event Storming

### Fase 2
- ✅ Escalabilidade e resiliência (Azure Container Instances)
- ✅ Dockerização (multi-stage Dockerfile)
- ✅ CI/CD (GitHub Actions com pipelines automatizados)
- ✅ Deploy na cloud (Azure)
- ✅ Monitoramento (Azure Monitor + Datadog)

---

**🎮 FIAP Cloud Games - Construindo o futuro dos jogos educacionais na nuvem!** ☁️
