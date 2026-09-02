# Desafio Técnico — TODO App (.NET + React)

**Público:** estagiários de desenvolvimento
**Duração sugerida:** 3 sprints de 1 semana
**Stack:** .NET 9 (ASP.NET Core) · EF Core · MySQL · React 19 + TypeScript (Vite) · Docker

---

## 1. Objetivo

Construir uma aplicação de gerenciamento de tarefas do zero, ponta a ponta: API REST em C# e SPA em React consumindo essa API.

O foco **não** é a complexidade do domínio — TODO é proposital, o problema é simples para que a atenção fique em:

- separação de responsabilidades (camadas, injeção de dependência);
- modelagem de dados e acesso via EF Core;
- desenho de contrato HTTP (verbos, status codes, payloads, erros);
- consumo de API no front, com tratamento de loading/erro/estado vazio;
- componentização e gerenciamento de estado em React;
- Git e code review.

> Este documento descreve **o que** deve existir e **como o sistema se comporta**. As decisões de implementação são de vocês — inclusive as erradas. Errar aqui é barato; a discussão no review é a parte que importa.

---

## 2. Regras do desafio

1. Nada de scaffolding automático de CRUD, template pronto de admin ou biblioteca que gere as telas.
2. Toda decisão técnica relevante (biblioteca escolhida, estrutura de pastas, abordagem de estado) deve estar registrada no `README.md` do repositório, com uma linha de justificativa.
3. Commits pequenos e com mensagem descritiva. Um commit chamado `ajustes` é motivo de review reprovado.
4. Branch por funcionalidade, PR para `main`, nunca commit direto na `main`.
5. Se travar por mais de 1 hora no mesmo problema, pare e pergunte. Travar faz parte; travar em silêncio a semana inteira não.

---

## 3. Requisitos funcionais

### 3.1 Entidades

**Task (Tarefa)**

| Campo | Tipo | Regras |
|---|---|---|
| `id` | Guid | gerado pelo servidor |
| `title` | string | obrigatório, 3–120 caracteres |
| `description` | string? | opcional, até 1000 caracteres |
| `status` | enum | `Pending`, `InProgress`, `Done` |
| `priority` | enum | `Low`, `Medium`, `High` |
| `dueDate` | DateTime? | opcional, não pode ser no passado na criação |
| `categoryId` | Guid? | opcional, referência a Category |
| `createdAt` | DateTime | UTC, definido pelo servidor |
| `updatedAt` | DateTime? | UTC, atualizado a cada alteração |
| `completedAt` | DateTime? | preenchido quando o status vira `Done`, limpo se sair de `Done` |

**Category (Categoria)**

| Campo | Tipo | Regras |
|---|---|---|
| `id` | Guid | gerado pelo servidor |
| `name` | string | obrigatório, único, 2–50 caracteres |
| `color` | string | obrigatório, hexadecimal (`#RRGGBB`) |

### 3.2 Histórias de usuário

**US-01 — Criar tarefa**
Como usuário, quero cadastrar uma tarefa informando título, descrição, prioridade, prazo e categoria.

*Critérios de aceite:*
- [ ] Título em branco ou com menos de 3 caracteres retorna erro de validação com mensagem legível;
- [ ] tarefa criada nasce com status `Pending`;
- [ ] `createdAt` é preenchido pelo servidor, nunca pelo cliente;
- [ ] resposta é `201 Created` com o header `Location` apontando para o recurso.

**US-02 — Listar tarefas**
Como usuário, quero ver minhas tarefas com filtro e ordenação.

*Critérios de aceite:*
- [ ] filtro por `status`, `priority`, `categoryId` e busca textual no título;
- [ ] ordenação por `dueDate`, `priority` ou `createdAt`, ascendente e descendente;
- [ ] resposta paginada (ver §4.4);
- [ ] combinação de filtros funciona em conjunto (`status=Pending&priority=High`);
- [ ] lista vazia retorna `200` com array vazio — **não** `404`.

**US-03 — Editar tarefa**
Como usuário, quero alterar os dados de uma tarefa existente.

*Critérios de aceite:*
- [ ] editar campos não informados no payload não os apaga (definam e documentem a semântica: PUT total ou PATCH parcial);
- [ ] `updatedAt` é atualizado;
- [ ] tarefa inexistente retorna `404`.

**US-04 — Concluir / reabrir tarefa**
Como usuário, quero marcar uma tarefa como concluída e poder reabri-la.

*Critérios de aceite:*
- [ ] endpoint dedicado de mudança de status, não um PUT genérico;
- [ ] ao concluir, `completedAt` recebe a data/hora atual em UTC;
- [ ] ao reabrir, `completedAt` volta a ser nulo;
- [ ] concluir uma tarefa já concluída não gera erro nem duplica efeito (idempotência).

**US-05 — Excluir tarefa**
Como usuário, quero remover uma tarefa.

*Critérios de aceite:*
- [ ] `204 No Content` em caso de sucesso;
- [ ] `404` se a tarefa não existir;
- [ ] o front pede confirmação antes de enviar a requisição.

**US-06 — Gerenciar categorias**
Como usuário, quero criar, listar, editar e excluir categorias.

*Critérios de aceite:*
- [ ] nome duplicado retorna `409 Conflict`;
- [ ] excluir categoria que possui tarefas vinculadas **não** apaga as tarefas — decidam entre bloquear a exclusão (`409`) ou desvincular, e justifiquem no README;
- [ ] cor inválida retorna erro de validação.

**US-07 — Resumo**
Como usuário, quero ver um resumo com total de tarefas, quantas estão pendentes, em andamento, concluídas e quantas estão atrasadas.

*Critérios de aceite:*
- [ ] "atrasada" = `dueDate` no passado e status diferente de `Done`;
- [ ] o cálculo é feito no banco, não trazendo todas as tarefas para a memória da API.

---

## 4. Backend (C# / .NET)

### 4.1 Organização

Estrutura sugerida — não é a única possível, mas quero a separação entre camadas explícita:

```
src/
  Todo.Api/            → endpoints, middlewares, configuração, DI
  Todo.Application/    → casos de uso, DTOs, validações, interfaces
  Todo.Domain/         → entidades, enums, regras de negócio
  Todo.Infrastructure/ → DbContext, EF configurations, repositórios, migrations
tests/
  Todo.Tests/
```

Regra que não se negocia: **`Todo.Domain` não referencia nenhum dos outros projetos.** As dependências apontam para dentro. Se o domínio precisar de algo externo, isso vira uma interface no domínio e a implementação fica na infraestrutura.

### 4.2 Pontos de atenção

- **Entidade nunca sai da API.** Requisição e resposta usam DTOs próprios. `Task` do domínio não é serializada direto.
- **Validação** com FluentValidation ou Data Annotations — escolham um e usem de forma consistente.
- **Datas sempre em UTC.** Conversão para o fuso do usuário é responsabilidade do front.
- **Async/await do endpoint até o banco.** Nada de `.Result` ou `.Wait()`.
- **Injeção de dependência** via construtor. Nada de `new` de repositório dentro do caso de uso.
- **Migrations versionadas** no repositório. O banco é criado por migration, não por script manual.
- Configuração sensível (connection string) via `appsettings.Development.json` local + variáveis de ambiente. Nada de senha commitada.

### 4.3 Contrato da API

| Método | Rota | Descrição | Sucesso |
|---|---|---|---|
| `GET` | `/api/tasks` | lista paginada com filtros | `200` |
| `GET` | `/api/tasks/{id}` | detalhe | `200` / `404` |
| `POST` | `/api/tasks` | cria | `201` |
| `PUT` | `/api/tasks/{id}` | atualiza | `204` / `404` |
| `PATCH` | `/api/tasks/{id}/status` | altera status | `204` / `404` |
| `DELETE` | `/api/tasks/{id}` | remove | `204` / `404` |
| `GET` | `/api/categories` | lista | `200` |
| `POST` | `/api/categories` | cria | `201` / `409` |
| `PUT` | `/api/categories/{id}` | atualiza | `204` / `404` / `409` |
| `DELETE` | `/api/categories/{id}` | remove | `204` / `404` / `409` |
| `GET` | `/api/tasks/summary` | resumo (US-07) | `200` |

**Query string de `GET /api/tasks`:**

```
?status=Pending
&priority=High
&categoryId={guid}
&search=texto
&sortBy=dueDate
&sortDirection=asc
&page=1
&pageSize=20
```

### 4.4 Formato de resposta paginada

```json
{
  "items": [ ... ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 137,
  "totalPages": 7
}
```

`pageSize` deve ter um teto (sugestão: 100). Se o cliente pedir 10.000, a API entrega o teto — não deixem o cliente decidir quanto o banco vai sofrer.

### 4.5 Formato de erro

Padronizem em `ProblemDetails` (RFC 7807), que o ASP.NET Core já suporta nativamente:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Erro de validação",
  "status": 400,
  "errors": {
    "title": ["O título deve ter no mínimo 3 caracteres."]
  }
}
```

Um middleware de tratamento de exceções centraliza isso. Não quero `try/catch` repetido em cada endpoint, e nem stack trace vazando para o cliente em produção.

### 4.6 Testes

Mínimo obrigatório:
- testes unitários das regras de negócio do domínio (transições de status, cálculo de atraso);
- testes unitários dos validadores;
- pelo menos um teste de integração de endpoint usando `WebApplicationFactory` com banco em memória ou container.

---

## 5. Frontend (React + TypeScript)

### 5.1 Organização sugerida

```
src/
  api/         → client HTTP e funções por recurso
  components/  → componentes reutilizáveis e burros
  features/    → tasks/, categories/ — componentes com regra da feature
  hooks/       → hooks customizados
  types/       → tipos e enums espelhando o contrato da API
  pages/       → composição das telas
```

### 5.2 Telas

1. **Lista de tarefas** — filtros, ordenação, paginação, ações rápidas (concluir/excluir).
2. **Formulário de tarefa** — criação e edição, modal ou página (decisão de vocês).
3. **Gerenciamento de categorias** — CRUD simples.
4. **Painel de resumo** — os números da US-07, no topo da listagem ou em tela própria.

### 5.3 Pontos de atenção

- **TypeScript sem `any`.** Os tipos do front espelham os DTOs da API. Se aparecer `any`, é porque a modelagem não foi feita.
- **Três estados sempre:** carregando, erro e vazio. Uma tela que só trata o caminho feliz está incompleta.
- **Erro da API é exibido ao usuário** com a mensagem que veio do `ProblemDetails`, não um `alert("erro")` genérico.
- **Validação no front espelha a do back**, mas não a substitui. O servidor valida sempre.
- **Debounce na busca textual** — não disparem uma requisição por tecla digitada.
- **Componentes pequenos.** Se um arquivo passou de ~150 linhas, provavelmente há um componente escondido ali dentro.
- **Estado do servidor ≠ estado da UI.** Podem usar React Query/TanStack Query ou `fetch` + hooks próprios; se optarem por hooks próprios, escrevam um `useTasks` reutilizável em vez de repetir `useEffect` em cada tela.
- **Nada de chave de configuração hardcoded.** URL da API vem de variável de ambiente do Vite.

---

## 6. Infraestrutura

- `docker-compose.yml` na raiz subindo MySQL (e opcionalmente a API).
- `README.md` com: pré-requisitos, passo a passo para subir o projeto, como rodar as migrations e como rodar os testes.
- O critério: alguém que nunca viu o projeto clona o repositório e coloca tudo no ar seguindo só o README.

---

## 7. Entregas por sprint

### Sprint 1 — Backend base
- [ ] Solução criada com os 4 projetos e as referências corretas
- [ ] Entidades e enums do domínio
- [ ] `DbContext`, configurações do EF e primeira migration aplicada
- [ ] CRUD de Category completo
- [ ] CRUD de Task sem filtros (`GET` simples)
- [ ] Swagger funcionando
- [ ] `docker-compose` subindo o MySQL

### Sprint 2 — Backend completo + front inicial
- [ ] Filtros, ordenação e paginação em `GET /api/tasks`
- [ ] `PATCH` de status com a regra de `completedAt`
- [ ] Endpoint de resumo
- [ ] Middleware de erro com `ProblemDetails`
- [ ] Testes unitários do domínio e dos validadores
- [ ] Projeto React criado, camada de API tipada, listagem de tarefas exibindo dados reais

### Sprint 3 — Front completo
- [ ] Formulário de criação e edição
- [ ] Filtros e busca integrados à API
- [ ] Tela de categorias
- [ ] Painel de resumo
- [ ] Tratamento de loading, erro e vazio em todas as telas
- [ ] README finalizado
- [ ] Apresentação de 15 minutos: demo + decisões técnicas tomadas

---

## 8. Definition of Done

Uma funcionalidade só está pronta quando:

- [ ] atende a todos os critérios de aceite da história;
- [ ] compila sem warnings novos;
- [ ] não tem código comentado nem `Console.WriteLine`/`console.log` esquecido;
- [ ] tem teste onde a especificação exige;
- [ ] passou por PR revisado e aprovado;
- [ ] o README reflete qualquer mudança de setup.

---

## 9. Critérios de avaliação

| Critério | Peso |
|---|---|
| Requisitos funcionais atendidos | 30% |
| Organização do código e separação de camadas | 20% |
| Qualidade do contrato HTTP (status, payloads, erros) | 15% |
| Qualidade do front (tipagem, estados, componentização) | 15% |
| Testes | 10% |
| Git, README e capacidade de explicar as decisões | 10% |

O último item costuma ser subestimado. Saber justificar por que escolheu uma abordagem vale mais do que ter escolhido a "certa" por acaso.

---

## 10. Desafios opcionais

Para quem terminar antes do prazo, em ordem crescente de dificuldade:

1. Soft delete com lixeira e restauração de tarefas.
2. Subtarefas (checklist dentro de uma tarefa).
3. Autenticação com JWT e tarefas por usuário.
4. Tarefas recorrentes (diária, semanal, mensal).
5. Log estruturado com Serilog e um health check.
6. Pipeline no GitHub Actions rodando build e testes a cada PR.

---

## 11. Referências

- [Documentação do ASP.NET Core](https://learn.microsoft.com/aspnet/core)
- [EF Core — provider MySQL (Pomelo)](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
- [FluentValidation](https://docs.fluentvalidation.net)
- [React — documentação oficial](https://react.dev)
- [TanStack Query](https://tanstack.com/query/latest)
- [RFC 7807 — Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
- [HTTP status codes](https://developer.mozilla.org/docs/Web/HTTP/Status)
