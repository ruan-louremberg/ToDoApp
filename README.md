#  TODO App — Documentação do Projeto e Decisões de Arquitetura

##  1. Sobre o Projeto

O **TODO App** é uma aplicação de gerenciamento de tarefas desenvolvida para demonstrar a construção de uma solução full-stack utilizando .NET e React.

O projeto tem objetivo pedagógico e técnico, contemplando:

- Separação de responsabilidades em camadas;
- API RESTful desenvolvida com ASP.NET Core;
- Persistência com Entity Framework Core;
- Validação centralizada de entradas;
- Padronização de erros;
- Desenvolvimento de uma SPA com React e TypeScript;
- Versionamento de banco de dados por migrations;
- Integração entre frontend e backend.

### Funcionalidades

| História | Funcionalidade | Status |
|---|---|---|
| US-01 | Criar tarefas | Implementada |
| US-02 | Listar tarefas | Implementada |
| US-03 | Atualizar tarefas parcialmente | Implementada/em ajuste |
| US-04 | Concluir tarefas | Implementada |
| US-05 | Excluir tarefas | Implementada |
| US-06 | Gerenciar categorias | Implementada/em ajuste |
| US-07 | Resumo e estatísticas agregadas | Pendente |

A US-07 permanece em fase de planejamento/desenvolvimento e será implementada em uma próxima iteração.

---

##  2. Tech Stack & Dependências

| Área | Tecnologia | Justificativa |
|---|---|---|
| Backend | .NET 9 / ASP.NET Core | Plataforma moderna, performática e adequada para construção de APIs RESTful. |
| ORM | Entity Framework Core | Facilita o mapeamento objeto-relacional e a implementação de migrations. |
| Banco de dados | SQLite | Simplifica o desenvolvimento local, não exige infraestrutura externa e permite execução rápida da aplicação. |
| Migrations | EF Core Migrations | Mantém o schema versionado e reproduzível, substituindo o uso de `EnsureCreated`. |
| Frontend | React 19 | Permite construir uma interface baseada em componentes reutilizáveis. |
| Linguagem frontend | TypeScript | Adiciona tipagem estática e reduz erros de integração entre a SPA e a API. |
| Bundler/dev server | Vite | Oferece inicialização rápida e uma experiência de desenvolvimento simples. |
| Validação | FluentValidation | Centraliza regras de validação de entrada e evita duplicação entre controllers e casos de uso. |
| Resultados de aplicação | FluentResults | Representa sucessos e falhas de casos de uso sem depender exclusivamente de exceções. |
| Infraestrutura local | CLI do .NET e Node.js/Vite | Atualmente são as formas oficiais de execução local. |
| Containerização | Docker | Planejada como próximo passo de infraestrutura; ainda não está disponível como fluxo oficial de execução. |

---

##  3. Arquitetura e Organização das Camadas

A solução é organizada em quatro projetos principais:

```text
src/
├── Todo.Api/
├── Todo.Application/
├── Todo.Domain/
└── Todo.Infrastructure/

frontend/
```

### `Todo.Domain`

Contém o núcleo do negócio:

- Entidades;
- Regras de domínio;
- Propriedades relacionadas ao ciclo de vida das tarefas;
- Conceitos como `Completed` e `completedAt`.

O domínio não deve depender de ASP.NET Core, Entity Framework ou detalhes de infraestrutura. Esse isolamento facilita testes, manutenção e eventual substituição de tecnologias externas.

### `Todo.Application`

Representa os casos de uso da aplicação, incluindo:

- Criação, consulta, alteração e exclusão de tarefas;
- Operações de categorias;
- DTOs;
- Validações com `FluentValidation`;
- Resultados de operação com `FluentResults`.

Essa camada coordena o fluxo da aplicação sem conhecer detalhes específicos de banco de dados ou transporte HTTP.

### `Todo.Infrastructure`

Concentra detalhes externos e de persistência:

- `DbContext`;
- Configurações do Entity Framework Core;
- Mapeamentos das entidades;
- SQLite;
- Migrations;
- Implementações de repositórios e serviços de infraestrutura.

O schema do banco é gerenciado exclusivamente por migrations versionadas.

### `Todo.Api`

É a camada de entrada HTTP da aplicação:

- Controllers;
- Configuração de injeção de dependência;
- Pipeline HTTP;
- Middlewares;
- Conversão de falhas para `ProblemDetails`;
- Exposição das rotas `/api/tasks` e `/api/categories`.

### Frontend

A aplicação frontend é uma SPA React desenvolvida com TypeScript e Vite. A organização prioriza:

- Componentes reutilizáveis;
- Separação entre apresentação e acesso à API;
- Tipagem dos modelos;
- Atualização imediata do estado local após operações de escrita.

O estado é mantido no frontend de forma local, sem adoção de uma biblioteca global adicional. Essa escolha é suficiente para o escopo atual e evita complexidade prematura.

---

##  4. Decisões Técnicas e Racional de Desenvolvimento

### 4.1 SQLite e EF Core Migrations

Foi escolhido SQLite para reduzir o atrito no desenvolvimento local. A aplicação pode ser executada sem configurar um servidor de banco de dados separado.

O schema é criado e alterado por **EF Core Migrations**, que são versionadas junto ao código. Essa abordagem foi escolhida porque:

- Mantém o histórico de alterações do banco;
- Permite reproduzir a estrutura em diferentes ambientes;
- Torna as alterações auditáveis;
- Evita a abordagem limitada de `EnsureCreated`.

A conexão com o banco deve ser configurada por variável de ambiente ou configuração local da aplicação.

### 4.2 Rota plural para tarefas

A rota oficial de tarefas é:

```text
/api/tasks
```

O uso do plural segue a convenção de APIs REST, em que a rota representa uma coleção de recursos.

### 4.3 Uso de `PATCH`

A atualização de tarefas utiliza `PATCH`, pois a operação representa uma alteração parcial do recurso.

Essa decisão é adequada ao comportamento da SPA React: o usuário normalmente altera apenas um campo, como:

- Título;
- Categoria;
- Data de vencimento;
- Estado da tarefa.

Não é necessário reenviar a entidade completa para alterar uma única propriedade.

Após a alteração, a API retorna o objeto atualizado com `200 OK`. Isso permite que o frontend sincronize o estado local imediatamente, sem realizar uma nova requisição `GET`.

### 4.4 Retorno temporário de `200 OK`

Durante a etapa de integração e validação do frontend, endpoints de criação e edição podem retornar `200 OK` com o objeto no corpo da resposta.

Essa escolha facilita:

- Visualização do resultado da operação;
- Atualização imediata da interface;
- Testes manuais;
- Validação do contrato entre frontend e backend.

Como evolução natural do contrato HTTP, os pontos pendentes podem ser ajustados para:

- `201 Created` em criações;
- `204 No Content` em operações que não precisem retornar conteúdo.

### 4.5 Validação com FluentValidation

`FluentValidation` é a única fonte oficial para validação de payloads de entrada.

As regras incluem, conforme o caso de uso:

- Obrigatoriedade de campos;
- Tamanho mínimo e máximo do título;
- Formatos válidos;
- Regras relacionadas a categorias;
- Regras de negócio da aplicação.

A centralização evita duplicar regras em controllers, entidades e frontend.

### 4.6 Result Pattern com FluentResults

`FluentResults` é utilizado para representar resultados de casos de uso e falhas esperadas do domínio.

A abordagem diferencia:

- Operações bem-sucedidas;
- Erros de validação;
- Recursos inexistentes;
- Conflitos de negócio;
- Falhas inesperadas.

Isso torna o fluxo de aplicação explícito e facilita a conversão das falhas para respostas HTTP padronizadas.

### 4.7 Tratamento de erros com ProblemDetails

Todos os erros da API são centralizados por middleware e retornados no formato `ProblemDetails`, conforme a RFC 7807.

Essa padronização permite que o frontend consuma uma estrutura única independentemente da origem do erro, incluindo respostas:

- `400 Bad Request`;
- `404 Not Found`;
- `409 Conflict`;
- `500 Internal Server Error`.

### 4.8 Exclusão de categorias com tarefas vinculadas

Ao excluir uma categoria, as tarefas relacionadas não são excluídas e a propriedade `CategoryId` é desvinculada, assumindo valor nulo.

A estratégia é equivalente a `Set Null`.

Essa decisão foi tomada para:

- Evitar perda de tarefas;
- Preservar o histórico dos dados;
- Não bloquear a ação do usuário com `409 Conflict`;
- Permitir que as tarefas continuem existindo sem categoria.

### 4.9 Convenção `Completed` e `completedAt`

O estado da tarefa utiliza o termo `Completed`, em vez de `Done`.

A escolha mantém alinhamento semântico com `completedAt`, propriedade que registra quando a tarefa foi concluída. `Completed` representa claramente um estado do ciclo de vida da tarefa e mantém uma nomenclatura consistente no domínio.

### 4.10 Campos opcionais e limpeza de valores nulos

Existe um débito técnico relacionado à semântica de limpeza explícita dos campos opcionais `dueDate` e `categoryId`.

A estratégia atual de atualização parcial ainda será aprimorada para diferenciar claramente:

- Campo não enviado: manter o valor atual;
- Campo enviado com valor: atualizar;
- Campo enviado como nulo: limpar o valor existente.

A recomendação futura é utilizar um DTO ou contrato de patch que represente explicitamente esses três estados.

### 4.11 Paginação

As listagens devem utilizar paginação para evitar o carregamento indiscriminado de todos os registros.

O parâmetro `pageSize` deve possuir limite máximo definido no backend, impedindo que o cliente solicite volumes excessivos de dados. O servidor deve aplicar o limite mesmo quando o cliente informar um valor maior.

A paginação também melhora:

- Tempo de resposta;
- Consumo de memória;
- Experiência da SPA;
- Escalabilidade futura.

### 4.12 Docker

A execução oficial atual ocorre diretamente pela CLI do .NET e pelo Node.js/Vite.

Dockerfile e Docker Compose estão mapeados como próximo passo de infraestrutura. Portanto, comandos Docker não são considerados atualmente o fluxo principal ou validado de execução do projeto.

---

##  5. Contrato da API e Formato de Erros

### Tarefas

| Método | Rota | Descrição | Status previstos |
|---|---|---|---|
| `GET` | `/api/tasks` | Lista tarefas, com suporte à paginação | `200 OK`, `400 Bad Request` |
| `GET` | `/api/tasks/{id}` | Busca uma tarefa por identificador | `200 OK`, `404 Not Found` |
| `POST` | `/api/tasks` | Cria uma tarefa | `200 OK` atualmente; evolução para `201 Created` |
| `PATCH` | `/api/tasks/{id}` | Atualiza parcialmente uma tarefa | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/tasks/{id}` | Exclui uma tarefa | `200 OK` ou `204 No Content`, conforme o endpoint |

### Categorias

| Método | Rota | Descrição | Status previstos |
|---|---|---|---|
| `GET` | `/api/categories` | Lista categorias | `200 OK` |
| `GET` | `/api/categories/{id}` | Busca uma categoria por identificador | `200 OK`, `404 Not Found` |
| `POST` | `/api/categories` | Cria uma categoria | `200 OK` atualmente; evolução para `201 Created` |
| `DELETE` | `/api/categories/{id}` | Exclui uma categoria e desvincula suas tarefas | `200 OK` ou `204 No Content`, `404 Not Found` |

Os códigos efetivamente retornados devem permanecer alinhados às implementações dos controllers e ao middleware global de erros.

### Exemplo de `ProblemDetails`

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Validation error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/tasks",
}
```

A estrutura permite que o frontend trate erros de forma uniforme, exibindo mensagens gerais e erros específicos por campo.

---

### Para rodar

```powershell
dotnet tool install --global dotnet-ef
```

### 1. Clonar o repositório

```powershell
git clone https://github.com/ruan-louremberg/ToDoApp.git
cd ToDoApp
```

### 2. Executar as migrations

Na raiz da solução:

```powershell
dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api
```

Esse comando cria ou atualiza o banco SQLite conforme as migrations versionadas no projeto.

### 3. Executar a API

```powershell
cd src/Todo.Api
dotnet restore
dotnet run
```

A URL da API será exibida no terminal pelo ASP.NET Core.

### 4. Executar o frontend

Em outro terminal:

```powershell
cd frontend
npm install
npm run dev
```

O Vite exibirá a URL local da aplicação, normalmente:

```text
http://localhost:5173
```

A URL da API deve estar configurada conforme o mecanismo de configuração utilizado pelo frontend.

### Docker

A aplicação ainda não possui um fluxo oficial de execução com Docker. Dockerfile e Docker Compose estão planejados como próximo passo de infraestrutura.

---

##  7. Testes e Débitos Técnicos Mapeados

### Executar os testes

Na raiz da solução:

```powershell
dotnet test
```

Para obter informações detalhadas:

```powershell
dotnet test --logger "console;verbosity=detailed"
```

### Débitos técnicos conhecidos

- Implementar a US-07, com resumo e estatísticas agregadas;
- Aprimorar o contrato de atualização parcial para permitir a limpeza explícita de `dueDate` e `categoryId`;
- Revisar os endpoints que ainda retornam `200 OK`, adotando `201 Created` e `204 No Content` quando apropriado;
- Finalizar o Dockerfile e o Docker Compose;
- Validar e documentar integralmente os limites e metadados da paginação;
- Ampliar a cobertura de testes de integração da API;
- Manter o contrato documentado sincronizado com as rotas reais dos controllers.

---

##  Considerações Finais

O projeto prioriza clareza arquitetural, baixo atrito no desenvolvimento local e contratos previsíveis entre API e frontend.

As principais escolhas — SQLite, EF Core Migrations, `PATCH`, FluentValidation, FluentResults, `ProblemDetails` e desvinculação de categorias — foram feitas para preservar a produtividade durante o desenvolvimento e permitir a evolução futura da aplicação.