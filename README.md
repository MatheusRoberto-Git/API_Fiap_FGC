# 🎮 FIAP Cloud Games (FCG) - API

> **Tech Challenge - Fase 1**  
> API REST em .NET 8 para gerenciamento de usuários e biblioteca de jogos, aplicando Domain-Driven Design (DDD) e Clean Architecture.

---

## 📋 Índice

- [🎯 Objetivos](#-objetivos)
- [🏗️ Arquitetura](#️-arquitetura)
- [🚀 Como Executar](#-como-executar)
- [📚 Endpoints da API](#-endpoints-da-api)
- [🔐 Autenticação JWT](#-autenticação-jwt)
- [🧪 Testes TDD](#-testes-tdd)
- [📖 Event Storming](#-event-storming)
- [🛠️ Tecnologias](#️-tecnologias)
- [👥 Equipe](#-equipe)

---

## 🎯 Objetivos

### Objetivo Geral
Desenvolver um **MVP da plataforma FIAP Cloud Games**, uma API para gestão de usuários e jogos que servirá como base para futuras funcionalidades de matchmaking e gerenciamento de servidores.

### Objetivos Específicos
- ✅ **Implementar Clean Architecture** com DDD
- ✅ **Criar API REST** em .NET 8 com documentação Swagger
- ✅ **Garantir autenticação segura** via JWT
- ✅ **Aplicar TDD** em pelo menos um módulo
- ✅ **Separar responsabilidades** entre usuários e administradores
- ✅ **Documentar arquitetura** com Event Storming

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
├── FGC.Domain/              # Regras de negócio e entidades
├── FGC.Application/         # Casos de uso e orquestração
├── FGC.Infrastructure/      # Acesso a dados e serviços externos
├── FGC.Presentation/        # Controllers e modelos de API
└── FGC.Domain.Tests/        # Testes unitários com TDD
```

---

## 🚀 Como Executar

### Pré-requisitos
- **.NET 8 SDK** ou superior
- **IDE**: Visual Studio 2022 ou VS Code
- **Git** para clonagem do repositório

### 1. Clonar o Repositório
```bash
git clone [URL_DO_REPOSITORIO]
cd API_Fiap_FGC
```

### 2. Restaurar Dependências
```bash
dotnet restore
```

### 3. Executar a Aplicação
```bash
# Executar em modo desenvolvimento
dotnet run --project FGC.Presentation

# OU executar com hot reload
dotnet watch run --project FGC.Presentation
```

### 4. Acessar a API
- **Swagger UI**: `https://localhost:61043`
- **API Base**: `https://localhost:61043/api`
- **Health Check**: `https://localhost:61043/health`

### 5. Banco de Dados
- **Desenvolvimento**: In-Memory Database (automático)
- **Produção**: SQL Server (configurar connection string)

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
GET  /api/users/{id}/profile  # Obter perfil do usuário
PUT  /api/users/{id}/change-password # Alterar senha
```

### 🔐 Endpoints Protegidos (JWT Required)

#### Administração (Role: Admin)
```http
POST /api/admin/create        # Criar novo administrador
PUT  /api/admin/promote       # Promover usuário a admin
PUT  /api/admin/deactivate/{id} # Desativar usuário
GET  /api/admin/me            # Informações do admin logado
```

---

## 🔐 Autenticação JWT

### Como Autenticar

#### 1. Registrar Usuário
```json
POST /api/users/register
{
  "email": "usuario@fgc.com",
  "password": "MinhaSenh@123",
  "name": "Nome do Usuário"
}
```

#### 2. Fazer Login
```json
POST /api/auth/login
{
  "email": "usuario@fgc.com",
  "password": "MinhaSenh@123"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "12345678-1234-1234-1234-123456789abc",
      "email": "usuario@fgc.com",
      "name": "Nome do Usuário",
      "role": "User"
    },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2024-01-01T14:00:00Z",
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
- **🟢 Público**: Registro, login, visualizar perfils
- **🔵 Usuário**: Alterar própria senha, acessar biblioteca (futuro)
- **🔴 Admin**: Gerenciar usuários, criar admins, acessar relatórios

---

## 🧪 Testes TDD

### Metodologia Aplicada
Implementei **Test-Driven Development (TDD)** seguindo o ciclo **RED-GREEN-REFACTOR**, conforme exigido pelo Tech Challenge.

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
| `Constructor_WithValidSimpleEmail` | Email básico válido (`user@fgc.com`) |
| `Constructor_WithValidEmailWithSubdomain` | Subdomínios (`admin@mail.fgc.com`) |
| `Constructor_WithValidEmailWithNumbers` | Números no email (`user123@fgc2024.com`) |
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
| `ImplicitConversion_ShouldReturnEmailValue` | Conversão implícita para string |

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

### DevOps
- **Git**: Controle de versão

---

## 👥 Equipe

### Desenvolvedor Principal
- **Nome**: [Matheus Roberto de Oliveira]
- **Discord**: [.meister.m]
- **GitHub**: [MatheusRoberto-Git]

### Papéis Técnicos
- **Solution Architect**: Desenho da arquitetura DDD
- **Backend Developer**: Implementação da API REST
- **DevOps Engineer**: Configuração de CI/CD
- **QA Engineer**: Implementação de testes TDD

---

## 📞 Suporte

### Links Úteis
- **Swagger**: `https://localhost:61043`
- **Health Check**: `https://localhost:61043/health`
- **Debug Users**: `https://localhost:61043/debug/users`

### Contato
- **Discord**: [.meister.m] para dúvidas técnicas
- **Email**: [matheus.pro2@hotmail.com] para questões do projeto

---

## 📄 Licença

Este projeto foi desenvolvido como parte do **Tech Challenge - FIAP** e é destinado exclusivamente para fins educacionais.

---

**🎮 FIAP Cloud Games - Construindo o futuro dos jogos educacionais!** 🚀