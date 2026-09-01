# ToDoApp 📝

Aplicação para gerenciamento de tarefas desenvolvida em **.NET 10** utilizando os princípios de **Clean Architecture (Arquitetura Limpa)**, **Domain-Driven Design (DDD)** e persistência em banco de dados **SQLite** com **Entity Framework Core**.

---

## 🏗️ Estrutura do Projeto

O projeto é estruturado em camadas desacopladas com responsabilidades bem definidas:

```
ToDoApp/
 ├── src/
 │    ├── Todo.Domain/            # Regras de Negócio, Entidades, Enums, Exceções e Interfaces de Repositório
 │    ├── Todo.Application/       # Casos de Uso (Use Cases) e fluxos da aplicação
 │    ├── Todo.Infrastructure/    # Persistência de Dados (EF Core SQLite), Mapeamentos e Injeção de Dependências
 │    └── Todo.Api/               # Controller/Endpoints da API Web, OpenAPI e Inicialização do Banco
 └── tests/
      └── Todo.Tests/             # Testes unitários da solução
```

---

## 🛠️ O que foi Aplicado e Modificado

### 1. Conceito e Criação de Exceções de Domínio (`DomainException`)
- Implementada a classe base `DomainException` em `ToDoApp.Domain.Exceptions` para representar falhas nas regras de negócio.
- A entidade `ToDo` valida o estado invariante (ex: garantia de título com pelo menos 3 caracteres e não vazio) lançando exceções de domínio amigáveis em vez de erros genéricos da linguagem.

### 2. Padrão Repository e Persistência com SQLite (`IToDoRepository`)
- **Domain (`IToDoRepository`):** Interface definida na camada de domínio contendo as operações assíncronas do contrato CRUD (`AddAsync`, `GetByIdAsync`, `GetAllAsync`, `UpdateAsync`, `DeleteAsync`).
- **Infrastructure (`TodoDbContext` e `TodoRepository`):**
  - Integração da biblioteca `Microsoft.EntityFrameworkCore.Sqlite`.
  - Configuração do `TodoDbContext` mapeando as entidades `ToDo` e `Category` com conversão de Enums para string e chaves primárias.
  - Implementação concreta de `IToDoRepository` em `TodoRepository`.
  - Método de extensão `AddInfrastructure` para registro de dependências e configuração da `ConnectionString`.

### 3. API e Inicialização Automática
- **API (`Program.cs` e `appsettings.json`):**
  - Adicionada a conexão `DefaultConnection` para o arquivo de banco SQLite (`Data Source=todo.db`).
  - Configurada a criação automática do banco de dados e suas tabelas no startup da API via `EnsureCreatedAsync()`.

### 4. Padronização de Namespaces (`ToDoApp.*`)
- Todos os arquivos da solução tiveram seus namespaces padronizados para o prefixo `ToDoApp.*`:
  - `ToDoApp.Domain.*`
  - `ToDoApp.Application.*`
  - `ToDoApp.Infrastructure.*`
  - `ToDoApp.Api.*`
  - `ToDoApp.Tests.*`

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download) ou superior instalado.

### 1. Compilar a Solução
```bash
dotnet build
```

### 2. Executar os Testes
```bash
dotnet test
```

### 3. Rodar a API
```bash
dotnet run --project src/Todo.Api
```

O banco de dados SQLite (`todo.db`) será gerado automaticamente na raiz da execução do projeto `Todo.Api` na primeira inicialização.

---

## 🧰 Tecnologias Utilizadas

- **C# / .NET 10**
- **Entity Framework Core 10 (SQLite)**
- **OpenAPI / Swagger**
- **xUnit** (para testes unitários)

