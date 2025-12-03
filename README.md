# 🎮 FIAP Cloud Games (FCG) - API

> **Tech Challenge - Fase 1**  
> API REST em .NET 8 para gerenciamento de usuários e biblioteca de jogos, aplicando Domain-Driven Design (DDD) e Clean Architecture.

> **Tech Challenge - Fase 2**  
> Evolução da API com CI/CD, Dockerização, Cloud Deployment e Monitoramento para garantir escalabilidade e resiliência.

> **Tech Challenge - Fase 3**  
> Migração para Microsserviços, integração com Elasticsearch, funções Serverless e API Gateway para eficiência operacional.

---

## 📋 Índice

- [🎯 Objetivos](#-objetivos)
- [🏗️ Arquitetura](#️-arquitetura)
- [🔷 Microsserviços](#-microsserviços)
- [🔍 Elasticsearch](#-elasticsearch)
- [⚡ Serverless](#-serverless)
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
Desenvolver a **plataforma FIAP Cloud Games** com arquitetura de microsserviços, busca otimizada com Elasticsearch e funções serverless, garantindo escalabilidade, modularidade e eficiência operacional.

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

### Objetivos Específicos - Fase 3
- ✅ **Migrar para Microsserviços** - 3 serviços independentes (Users, Games, Payments)
- ✅ **Implementar Elasticsearch** - Busca e indexação otimizada de jogos
- ✅ **Criar funções Serverless** - AWS Lambda/Azure Functions para processos assíncronos
- ✅ **Configurar API Gateway** - Gerenciamento centralizado de requisições
- ✅ **Implementar Event Sourcing** - Registro de mudanças de estado
- ✅ **Melhorar Observabilidade** - Logs e rastreamento distribuído (Traces)

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

### Arquitetura de Microsserviços - Fase 3

```
                                    ┌─────────────────┐
                                    │   API Gateway   │
                                    │  (Azure APIM)   │
                                    └────────┬────────┘
                                             │
                    ┌────────────────────────┼────────────────────────┐
                    │                        │                        │
                    ▼                        ▼                        ▼
           ┌───────────────┐        ┌───────────────┐        ┌───────────────┐
           │    Users      │        │    Games      │        │   Payments    │
           │ Microservice  │        │ Microservice  │        │ Microservice  │
           └───────┬───────┘        └───────┬───────┘        └───────┬───────┘
                   │                        │                        │
                   ▼                        ▼                        ▼
           ┌───────────────┐        ┌───────────────┐        ┌───────────────┐
           │   SQL Server  │        │ Elasticsearch │        │   SQL Server  │
           │   (Users DB)  │        │  (Games Index)│        │ (Payments DB) │
           └───────────────┘        └───────────────┘        └───────────────┘
                                           │
                                           ▼
                                    ┌───────────────┐
                                    │    Azure      │
                                    │   Functions   │
                                    │ (Serverless)  │
                                    └───────────────┘
```

### Contextos Delimitados (Bounded Contexts)
- **👤 User Management**: Cadastro, autenticação e gestão de usuários
- **🎮 Game Catalog**: Catálogo, busca e recomendação de jogos
- **💳 Payment Processing**: Processamento e status de transações
- **🎉 Promotion**: Sistema de promoções (futuro)

### Estrutura de Projetos
```
API_Fiap_FGC/
├── .github/workflows/       # CI/CD Pipelines
│   ├── ci.yml              # Continuous Integration
│   └── cd.yml              # Continuous Deployment
├── FGC.Domain/             # Regras de negócio e entidades
│   ├── UserManagement/     # Contexto de Usuários
│   ├── GameManagement/     # Contexto de Jogos (Fase 3)
│   └── PaymentManagement/  # Contexto de Pagamentos (Fase 3)
├── FGC.Application/        # Casos de uso e orquestração
├── FGC.Infrastructure/     # Acesso a dados e serviços externos
├── FGC.Presentation/       # Controllers e modelos de API
├── FGC.Domain.Tests/       # Testes unitários com TDD
├── FGC.Serverless/         # Azure Functions (Fase 3)
└── Dockerfile              # Imagem Docker multi-stage
```

---

## 🔷 Microsserviços

### Visão Geral

A aplicação foi dividida em **3 microsserviços principais**, cada um com responsabilidades bem definidas:

| Microsserviço | Responsabilidade | Endpoints Base |
|---------------|------------------|----------------|
| **Users** | Cadastro, login, gestão de perfis e administração | `/api/users`, `/api/auth`, `/api/admin` |
| **Games** | Listagem, busca, recomendação e gestão de jogos | `/api/games` |
| **Payments** | Processamento, status e histórico de transações | `/api/payments` |

### Microsserviço de Usuários (Users)

**Funcionalidades:**
- Cadastro de novos usuários
- Autenticação via JWT
- Gerenciamento de perfis
- Administração (promoção/demoção de admins)
- Ativação/Desativação de contas

**Eventos de Domínio:**
- `UserCreatedEvent`
- `UserAuthenticatedEvent`
- `PasswordChangedEvent`
- `UserDeactivatedEvent`
- `UserReactivatedEvent`
- `UserPromotedToAdminEvent`
- `AdminUserCreatedEvent`

### Microsserviço de Jogos (Games)

**Funcionalidades:**
- Listagem de jogos ativos
- Busca por título (integração Elasticsearch)
- Busca por categoria
- Criação de jogos (Admin)
- Atualização de preços (Admin)
- Recomendações baseadas em histórico
- Métricas de jogos populares

**Eventos de Domínio:**
- `GameCreatedEvent`
- `GamePriceUpdatedEvent`
- `GameDeactivatedEvent`

**Categorias Disponíveis:**
- Action, Adventure, RPG, Strategy, Sports
- Racing, Simulation, Puzzle, Horror, FPS
- MMORPG, Indie, Fighting, Platformer, Sandbox

### Microsserviço de Pagamentos (Payments)

**Funcionalidades:**
- Criação de pagamentos
- Processamento de transações
- Consulta de status
- Histórico por usuário
- Reembolsos (Admin)

**Estados de Pagamento:**
```
Pending → Processing → Completed
                    ↘ Failed
Completed → Refunded
Pending → Cancelled
```

**Eventos de Domínio:**
- `PaymentCreatedEvent`
- `PaymentProcessingEvent`
- `PaymentCompletedEvent`
- `PaymentFailedEvent`
- `PaymentRefundedEvent`
- `PaymentCancelledEvent`

**Métodos de Pagamento:**
- CreditCard, DebitCard, Pix
- BankSlip, PayPal, ApplePay, GooglePay

### Comunicação entre Microsserviços

```
┌──────────────┐     HTTP/REST      ┌──────────────┐
│    Users     │◄──────────────────►│    Games     │
└──────────────┘                    └──────────────┘
       │                                   │
       │          ┌──────────────┐         │
       └─────────►│   Payments   │◄────────┘
                  └──────────────┘
                         │
                         ▼
                  ┌──────────────┐
                  │    Events    │
                  │    (Async)   │
                  └──────────────┘
```

---

## 🔍 Elasticsearch

### Implementação

O Elasticsearch foi implementado para otimizar a busca e indexação dos dados de jogos.

### Índice de Jogos

```json
{
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "title": { 
        "type": "text",
        "analyzer": "standard",
        "fields": {
          "keyword": { "type": "keyword" }
        }
      },
      "description": { "type": "text" },
      "price": { "type": "float" },
      "category": { "type": "keyword" },
      "developer": { "type": "keyword" },
      "publisher": { "type": "keyword" },
      "releaseDate": { "type": "date" },
      "rating": { "type": "float" },
      "totalSales": { "type": "integer" },
      "isActive": { "type": "boolean" }
    }
  }
}
```

### Consultas Avançadas

**Busca por Título (Full-Text Search):**
```json
{
  "query": {
    "match": {
      "title": {
        "query": "adventure",
        "fuzziness": "AUTO"
      }
    }
  }
}
```

**Recomendações por Histórico:**
```json
{
  "query": {
    "bool": {
      "should": [
        { "term": { "category": "RPG" } },
        { "term": { "developer": "CD Projekt" } }
      ],
      "must_not": [
        { "terms": { "id": ["jogos-já-comprados"] } }
      ]
    }
  }
}
```

**Agregações - Jogos Mais Populares:**
```json
{
  "aggs": {
    "top_categories": {
      "terms": { "field": "category", "size": 10 }
    },
    "avg_rating_by_category": {
      "terms": { "field": "category" },
      "aggs": {
        "avg_rating": { "avg": { "field": "rating" } }
      }
    },
    "most_sold": {
      "top_hits": {
        "sort": [{ "totalSales": "desc" }],
        "size": 10
      }
    }
  }
}
```

### Endpoints de Busca

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/games/search?term=xxx` | Busca full-text por título |
| GET | `/api/games/category/{category}` | Filtro por categoria |
| GET | `/api/games/recommendations/{userId}` | Recomendações personalizadas |
| GET | `/api/games/popular` | Jogos mais vendidos |
| GET | `/api/games/top-rated` | Jogos melhor avaliados |

---

## ⚡ Serverless

### Azure Functions Implementadas

As funções serverless foram criadas para processos assíncronos, garantindo escalabilidade e eficiência.

### 1. ProcessPaymentFunction

**Trigger:** HTTP / Queue  
**Descrição:** Processa pagamentos de forma assíncrona

```csharp
[FunctionName("ProcessPayment")]
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
    [Queue("payment-processed")] IAsyncCollector<string> outputQueue)
{
    // Processa pagamento
    // Envia para fila de confirmação
}
```

### 2. SendNotificationFunction

**Trigger:** Queue (payment-processed)  
**Descrição:** Envia notificações após eventos

```csharp
[FunctionName("SendNotification")]
public async Task Run(
    [QueueTrigger("payment-processed")] string message,
    [SendGrid] IAsyncCollector<SendGridMessage> emails)
{
    // Envia email de confirmação
}
```

### 3. SyncElasticsearchFunction

**Trigger:** Timer (a cada 5 minutos) / Event  
**Descrição:** Sincroniza dados de jogos com Elasticsearch

```csharp
[FunctionName("SyncElasticsearch")]
public async Task Run(
    [TimerTrigger("0 */5 * * * *")] TimerInfo timer)
{
    // Sincroniza jogos novos/atualizados
}
```

### 4. GameRecommendationFunction

**Trigger:** HTTP  
**Descrição:** Gera recomendações personalizadas

```csharp
[FunctionName("GetRecommendations")]
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
{
    // Consulta Elasticsearch
    // Retorna recomendações
}
```

### Configuração de Triggers

| Função | Trigger | Configuração |
|--------|---------|--------------|
| ProcessPayment | HTTP + Queue | POST `/api/process-payment` |
| SendNotification | Queue | `payment-processed` queue |
| SyncElasticsearch | Timer | `0 */5 * * * *` (cada 5 min) |
| GameRecommendation | HTTP | GET `/api/recommendations` |

### API Gateway (Azure API Management)

**Funcionalidades:**
- Roteamento centralizado para microsserviços
- Rate limiting (proteção contra sobrecarga)
- Autenticação JWT centralizada
- Cache de responses
- Logging e analytics

**Configuração de Rotas:**

```yaml
apis:
  - name: users-api
    path: /api/users/*
    backend: https://fgc-users.azurewebsites.net
    
  - name: games-api
    path: /api/games/*
    backend: https://fgc-games.azurewebsites.net
    
  - name: payments-api
    path: /api/payments/*
    backend: https://fgc-payments.azurewebsites.net
    
  - name: serverless-api
    path: /api/functions/*
    backend: https://fgc-functions.azurewebsites.net
```

---

## 🚀 Como Executar

### Pré-requisitos
- **.NET 8 SDK** ou superior
- **Docker Desktop** (para containerização)
- **Azure CLI** (para deploy na cloud)
- **Elasticsearch** (local ou cloud)
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
  -e Elasticsearch__Url="http://localhost:9200" \
  --name fgc-api-local \
  fgc-api:local

# Testar
curl http://localhost:8080/health
```

### Executar Elasticsearch Local

```bash
# Com Docker
docker run -d --name elasticsearch \
  -p 9200:9200 -p 9300:9300 \
  -e "discovery.type=single-node" \
  -e "xpack.security.enabled=false" \
  elasticsearch:8.11.0

# Verificar
curl http://localhost:9200
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
- ✅ Deploy das Azure Functions
- ✅ Health check automático
- ✅ Notificação de deployment no Datadog

**Arquivo:** `.github/workflows/cd.yml`

### Infraestrutura Azure

**Recursos provisionados:**
- **Azure Container Registry** (`fgcregistry`) - Armazenamento de imagens Docker
- **Azure Container Instance** (`fgc-api-container`) - Hosting da aplicação
- **Azure SQL Database** (`fgc-database`) - Banco de dados
- **Azure Functions** (`fgc-functions`) - Funções serverless
- **Azure API Management** - API Gateway
- **Elasticsearch Service** - Busca e indexação
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
- 🔍 Traces distribuídos

**Configuração:**
- Service: `fgc-api`
- Environment: `production`
- Site: `datadoghq.com`

### Rastreamento Distribuído (Traces)

Com a arquitetura de microsserviços, implementamos tracing distribuído para acompanhar requisições entre serviços:

```
[API Gateway] → [Users Service] → [Database]
                      ↓
              [Payments Service] → [Queue] → [Function]
                      ↓
               [Games Service] → [Elasticsearch]
```

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

#### Jogos
```http
GET  /api/games               # Listar todos os jogos ativos
GET  /api/games/{id}          # Buscar jogo por ID
GET  /api/games/search?term=xxx # Buscar jogos por título
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

#### Jogos - Admin
```http
POST /api/games                  # Criar novo jogo
PUT  /api/games/{id}/price       # Atualizar preço do jogo
```

#### Pagamentos
```http
POST /api/payments               # Criar pagamento
GET  /api/payments/{id}          # Buscar pagamento por ID
GET  /api/payments/{id}/status   # Consultar status do pagamento
GET  /api/payments/user/{userId} # Listar pagamentos do usuário
POST /api/payments/{id}/process  # Processar pagamento pendente
POST /api/payments/{id}/refund   # Solicitar reembolso (Admin)
```

---

## 🔐 Autenticação JWT

### Como Autenticar

#### 1. Registrar Usuário
```bash
curl -X POST http://fgc-api-v1.eastus2.azurecontainer.io:8080/api/users/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@fgc.com",
    "password": "MinhaSenh@123",
    "name": "Nome do Usuário"
  }'
```

#### 2. Fazer Login
```bash
curl -X POST http://fgc-api-v1.eastus2.azurecontainer.io:8080/api/auth/login \
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
- **🟢 Público**: Registro, login, visualizar jogos
- **🔵 Usuário**: Comprar jogos, ver pagamentos, alterar senha
- **🔴 Admin**: Gerenciar usuários, criar jogos, processar reembolsos

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
[Política] Save: SQL Server + Elasticsearch
    ↓
[Evento] Jogo Cadastrado
    ↓
[Modelo Leitura] Catálogo de jogos
```

#### 💳 Fluxo de Compra de Jogo (Fase 3)
```
[Comando] Comprar Jogo
    ↓
[Política] Validar: Usuário autenticado
    ↓
[Política] Validar: Jogo disponível
    ↓
[Comando] Criar Pagamento
    ↓
[Evento] Pagamento Criado
    ↓
[Trigger] Azure Function: ProcessPayment
    ↓
[Política] Processar: Gateway de Pagamento
    ↓
[Evento] Pagamento Completado / Falhou
    ↓
[Trigger] Azure Function: SendNotification
    ↓
[Política] Enviar: Email de confirmação
```

### Contextos Identificados
- **User Management**: Usuários, autenticação, perfis
- **Game Catalog**: Jogos, preços, categorias, busca
- **Payment Processing**: Transações, status, reembolsos
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
- **Azure Functions**: Serverless
- **Azure API Management**: API Gateway
- **GitHub Actions**: CI/CD
- **Docker**: Containerização

### Busca e Indexação
- **Elasticsearch**: Busca full-text e agregações
- **NEST**: Cliente .NET para Elasticsearch

### Monitoramento
- **Azure Monitor**: Logs e métricas nativas
- **Datadog**: APM e eventos de deployment
- **Container Insights**: Telemetria de containers
- **Application Insights**: Traces distribuídos

### Banco de Dados
- **SQL Server**: Produção
- **In-Memory Database**: Desenvolvimento e testes

### Testes
- **xUnit**: Framework de testes unitários
- **FluentAssertions**: Assertions expressivas

### Arquitetura
- **Clean Architecture**: Separação de responsabilidades
- **Domain-Driven Design**: Modelagem do domínio
- **Microsserviços**: Serviços independentes
- **Event Sourcing**: Eventos de domínio
- **CQRS**: Command Query Responsibility Segregation

---

## 👥 Equipe

### Desenvolvedor Principal
- **Nome**: Matheus Roberto de Oliveira
- **Discord**: .meister.m
- **GitHub**: MatheusRoberto-Git

### Papéis Técnicos
- **Solution Architect**: Desenho da arquitetura DDD e Clean Architecture
- **Backend Developer**: Implementação da API REST e Microsserviços
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

### Fase 3
- ✅ **Microsserviços** - 3 serviços: Users, Games, Payments
- ✅ **Elasticsearch** - Indexação e busca avançada de jogos
- ✅ **Serverless** - Azure Functions para processos assíncronos
- ✅ **API Gateway** - Gerenciamento centralizado de requisições
- ✅ **Event Sourcing** - Registro de todas as mudanças de estado
- ✅ **Observabilidade** - Logs e rastreamento distribuído

---

**🎮 FIAP Cloud Games - Construindo o futuro dos jogos educacionais na nuvem!** ☁️
