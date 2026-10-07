# TODO App

Aplicação full-stack de gerenciamento de tarefas, construída com ASP.NET Core, Entity Framework Core, SQLite, React, TypeScript e Vite.

O projeto foi desenvolvido com foco em separação de responsabilidades, validação de dados, contratos HTTP previsíveis e uma interface simples para criação, edição, conclusão, exclusão e restauração de tarefas.

## Funcionalidades

- Criar tarefas;
- Listar tarefas com busca, filtros, ordenação e paginação;
- Editar parcialmente uma tarefa;
- Alterar o status de uma tarefa;
- Excluir tarefas com soft delete;
- Listar e restaurar tarefas excluídas;
- Criar, listar, editar, excluir e restaurar categorias;
- Desvincular tarefas quando uma categoria é excluída, sem apagar as tarefas;
- Exibir resumo agregado das tarefas;
- Mostrar erros da API no formato `ProblemDetails`;
- Manter os valores digitados no modal de edição quando uma validação falha.

## Tecnologias

| Área | Tecnologia |
|---|---|
| Backend | .NET 10 / ASP.NET Core |
| API | Controllers REST e OpenAPI |
| Persistência | Entity Framework Core 10 |
| Banco local | SQLite |
| Migrations | EF Core Migrations |
| Validação | FluentValidation |
| Resultados de casos de uso | FluentResults |
| Frontend | React 19 |
| Linguagem do frontend | TypeScript |
| Build e servidor frontend | Vite |
| Ícones | lucide-react |
| Lint frontend | Oxlint |
| Containers | Docker e Docker Compose |
| Testes backend | xUnit, Moq e Coverlet |

## Estrutura do projeto

```text
.
├── src/
│   ├── Todo.Api/
│   ├── Todo.Application/
│   ├── Todo.Domain/
│   └── Todo.Infrastructure/
├── tests/
│   └── Todo.Tests/
├── frontend/
├── docker-compose.yml
└── ToDoApp.slnx
```

### Backend

- `Todo.Domain`: entidades, enums, exceções, `Optional<T>` e interfaces de repositório. Não depende de ASP.NET Core ou Entity Framework.
- `Todo.Application`: DTOs, validadores, casos de uso e regras de orquestração da aplicação.
- `Todo.Infrastructure`: `TodoDbContext`, configurações do EF Core, SQLite, migrations e implementações dos repositórios.
- `Todo.Api`: composição da aplicação HTTP, controllers, CORS, OpenAPI, middleware de erros e injeção de dependências.

### Frontend

O frontend é uma SPA React. As responsabilidades estão separadas desta forma:

- `src/api`: clientes HTTP e tratamento de erros;
- `src/components`: componentes reutilizáveis e modais;
- `src/hooks`: estado assíncrono e operações de tarefas/categorias;
- `src/pages`: composição das telas;
- `src/types`: contratos TypeScript da API;
- `src/features`: funcionalidades específicas, como o resumo de tarefas.

## Arquitetura e decisões importantes

### Fluxo de uma requisição

```text
Frontend
  -> Controller
  -> Use case
  -> Interface de repositório
  -> Repositório EF Core
  -> SQLite
```

O domínio não conhece HTTP nem banco de dados. A API é responsável pelo transporte, a aplicação coordena os casos de uso e a infraestrutura implementa a persistência.

### Validação e erros

As entradas são validadas com FluentValidation. Falhas esperadas dos casos de uso são representadas com FluentResults e convertidas pelos controllers em `ProblemDetails`.

Uma resposta de erro possui este formato:

```json
{
  "status": 400,
  "title": "Erro de validação",
  "detail": "O prazo não pode estar no passado.",
  "instance": "/api/tasks/00000000-0000-0000-0000-000000000000",
  "traceId": "..."
}
```

O frontend lê o campo `detail` e exibe a mensagem devolvida pela API.

### Atualização parcial

As tarefas são atualizadas com `PATCH`. O contrato usa `Optional<T>` e um conversor JSON próprio para diferenciar:

- campo ausente: mantém o valor atual;
- campo enviado com valor: atualiza o campo;
- campo enviado como `null`: limpa o valor existente.

Essa diferença é importante principalmente para `dueDate` e `categoryId`.

### Soft delete

Excluir uma tarefa ou categoria não remove imediatamente o registro do banco. O registro recebe as informações de exclusão e deixa de aparecer nas listagens normais.

As rotas de lixeira permitem consultar e restaurar os registros. Ao excluir uma categoria, as tarefas relacionadas permanecem salvas e ficam sem categoria.

### Formulário de edição

Se a API rejeitar uma edição, o modal continua aberto e mantém os dados digitados. Por exemplo, ao alterar a descrição e informar um prazo no passado, a mensagem de validação aparece sem apagar a descrição.

No frontend, `useMemo` mantém estáveis os dados iniciais do formulário enquanto a tarefa selecionada não muda. Assim, uma nova renderização causada pela mensagem de erro não reinicializa os campos editados.

### Banco e migrations

O banco usado localmente é SQLite e o schema é controlado por migrations versionadas em `src/Todo.Infrastructure/Migrations`.

Além do comando manual de migration, a API executa `Database.MigrateAsync()` durante a inicialização, aplicando migrations pendentes automaticamente.

## API

### Tarefas

| Método | Rota | Descrição | Respostas principais |
|---|---|---|---|
| `GET` | `/api/tasks` | Lista tarefas com filtros e paginação | `200`, `400` |
| `POST` | `/api/tasks` | Cria uma tarefa | `201`, `400` |
| `PATCH` | `/api/tasks/{id}` | Atualiza parcialmente uma tarefa | `200`, `400`, `404` |
| `PATCH` | `/api/tasks/{id}/status` | Altera o status da tarefa | `200`, `400`, `404` |
| `DELETE` | `/api/tasks/{id}` | Move uma tarefa para a lixeira | `204`, `404` |
| `GET` | `/api/tasks/trash` | Lista tarefas excluídas | `200` |
| `PATCH` | `/api/tasks/{id}/restore` | Restaura uma tarefa | `204`, `404` |
| `GET` | `/api/tasks/summary` | Retorna o resumo das tarefas | `200` |

Os filtros da listagem incluem busca, status, prioridade, categoria, ordenação, página e tamanho da página. O backend limita `page` e `pageSize` para evitar solicitações inválidas ou excessivamente grandes.

### Categorias

| Método | Rota | Descrição | Respostas principais |
|---|---|---|---|
| `GET` | `/api/categories` | Lista categorias | `200`, `400` |
| `POST` | `/api/categories` | Cria uma categoria | `200`, `409` |
| `PATCH` | `/api/categories/{id}` | Atualiza uma categoria | `200`, `400`, `404`, `409` |
| `DELETE` | `/api/categories/{id}` | Exclui uma categoria | `200`, `404` |
| `GET` | `/api/categories/trash` | Lista categorias excluídas | `200` |
| `PATCH` | `/api/categories/{id}/restore` | Restaura uma categoria | `204`, `404`, `409` |

## Pré-requisitos

Para executar localmente:

- .NET SDK 10;
- Node.js e npm;
- Git.

Para executar com containers:

- Docker Desktop com Docker Compose.

## Configuração

### Banco de dados

Por padrão, a API usa:

```text
Data Source=todo.db
```

Essa configuração está em `src/Todo.Api/appsettings.json`. Ela pode ser substituída pela configuração `ConnectionStrings:DefaultConnection`.

No Docker Compose, o banco fica em `/app/data/todo.db` e é persistido no volume `todo-data`.

### Frontend

O frontend precisa da variável `VITE_API_URL`. Crie ou confirme o arquivo `frontend/.env`:

```env
VITE_API_URL=http://localhost:5080
```

Não coloque a URL da API diretamente nos componentes ou nos clientes HTTP.

## Execução local

### 1. Clonar o repositório

```powershell
git clone https://github.com/ruan-louremberg/ToDoApp.git
cd ToDoApp
```

### 2. Restaurar dependências

Na raiz do repositório:

```powershell
dotnet restore
cd frontend
npm install
cd ..
```

### 3. Instalar o Entity Framework CLI

Esse passo é necessário apenas se o comando `dotnet ef` ainda não estiver instalado:

```powershell
dotnet tool install --global dotnet-ef
```

### 4. Aplicar migrations manualmente

```powershell
dotnet ef database update `
  --project src/Todo.Infrastructure `
  --startup-project src/Todo.Api
```

Esse comando cria ou atualiza o arquivo SQLite. A API também aplica migrations pendentes ao iniciar.

### 5. Executar a API

Em um terminal:

```powershell
dotnet run --project src/Todo.Api
```

### 6. Executar o frontend

Em outro terminal:

```powershell
cd frontend
npm run dev
```

O frontend normalmente ficará disponível em `http://localhost:5173`.

Em ambiente de desenvolvimento, a API disponibiliza:

- OpenAPI: `http://localhost:5080/openapi/v1.json`;
- Scalar: `http://localhost:5080/scalar`.

## Execução com Docker Compose

Na raiz do projeto:

```powershell
docker compose up --build
```

Endereços:

- Frontend: `http://localhost:5173`;
- API: `http://localhost:5080`;
- Scalar: `http://localhost:5080/scalar`.

Para parar os containers:

```powershell
docker compose down
```

O volume `todo-data` preserva o banco SQLite entre reinicializações. Para remover os containers e o volume:

```powershell
docker compose down -v
```

## Build, lint e testes

### Backend

Compilar a solução:

```powershell
dotnet build ToDoApp.slnx
```

Executar todos os testes:

```powershell
dotnet test
```

Executar somente o projeto de testes:

```powershell
dotnet test tests/Todo.Tests/Todo.Tests.csproj
```

Filtrar uma classe de testes:

```powershell
dotnet test tests/Todo.Tests/Todo.Tests.csproj `
  --filter "FullyQualifiedName~CreateTaskUseCaseTests"
```

Os testes atuais cobrem entidades, validadores e casos de uso de tarefas e categorias.

### Frontend

Na pasta `frontend`:

```powershell
npm run build
npm run lint
```

O projeto não possui atualmente uma suíte automatizada de testes para o frontend.

## Dockerfiles e configuração de portas

O arquivo `docker-compose.yml` inicia dois serviços:

- `api`: constrói `src/Todo.Api/Dockerfile`, expõe a porta `5080` e usa o volume `todo-data`;
- `frontend`: constrói `frontend/Dockerfile`, expõe a porta `5173` e usa `VITE_API_URL=http://localhost:5080`.

## Próximas melhorias

- Ampliar a cobertura de testes de integração da API;
- Adicionar testes automatizados para o frontend;
- Manter a documentação de contratos sincronizada com as alterações dos controllers;
- Avaliar melhorias de observabilidade e logs para ambientes de produção;
- Revisar configurações de produção, incluindo HTTPS, secrets e persistência externa.
