# Guia de Frontend — passo a passo simples

> Esse guia explica, de um jeito direto, como organizar o código do frontend e o que cada um pode ir fazendo. Sempre que tiver dúvida, pergunta — não precisa travar sozinho.

## 1. Como organizar as pastas

```
src/
  api/         → funções que conversam com a API (fetch)
  components/  → peças pequenas de tela (botão, card, aviso)
  features/    → telas de cada assunto (tarefas, categorias)
  hooks/       → funções reutilizáveis de estado
  types/       → o "formato" dos dados (tipos)
  pages/       → junta tudo pra formar a tela final
```

**Como saber onde colocar um componente:**

- Só mostra algo na tela, sem saber nada sobre "tarefa" ou "categoria"? → `components/`
- Sabe lidar com tarefa ou categoria (formulário, filtro)? → `features/`
- É a tela inteira? → `pages/`

Hoje quase tudo está dentro de um arquivo só (`TaskListPage.tsx`). A ideia é ir separando aos poucos.

## 2. Exemplo prático: como criar uma peça nova

### Tipo primeiro

Antes de criar tela, cria o "formato" do dado em `types/`:

```ts
// types/task.ts
export type TaskStatus = "Pending" | "InProgress" | "Done";

export interface Task {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
}
```

Isso evita usar `any` (deixar o dado "sem tipo"), que é proibido no desafio.

### Função de API

Uma função por ação, simples assim:

```ts
// api/tasks.ts
export async function deleteTask(id: string) {
  const response = await fetch(`${API_URL}/api/tasks/${id}`, {
    method: "DELETE",
  });
  if (!response.ok) throw new Error("Erro ao excluir tarefa");
}
```

A URL da API sempre vem de `import.meta.env.VITE_API_URL` — nunca digitem a URL fixa no código.

### Componente simples

```tsx
// components/EmptyState.tsx
export function EmptyState({ message }: { message: string }) {
  return <p>{message}</p>;
}
```

Componentes assim (`EmptyState`, `LoadingState`, `ErrorState`, `StatusBadge`) podem ser tirados de dentro de `TaskListPage.tsx` — hoje está tudo escrito ali dentro, e dá pra separar em arquivos pequenos.

## 3. Sempre mostrar 3 coisas: carregando, erro e vazio

Toda tela que busca dado da API precisa cobrir essas 3 situações, não só quando dá tudo certo:

```tsx
{loading && <p>Carregando...</p>}
{!loading && error && <p>{error}</p>}
{!loading && !error && items.length === 0 && <p>Nenhuma tarefa ainda.</p>}
{!loading && !error && items.length > 0 && <Lista items={items} />}
```

## 4. Onde entra o CSS

Hoje **não existe nenhum CSS no projeto** — as classes (`className="filters"`, etc) não têm nenhum estilo escrito em lugar nenhum.

Forma mais simples de resolver, um arquivo de estilo do lado de cada componente:

```
components/
  EmptyState.tsx
  EmptyState.module.css
```

```css
/* EmptyState.module.css */
p {
  color: #888;
  text-align: center;
}
```

```tsx
import styles from "./EmptyState.module.css";
```

E um arquivo único de estilo geral, importado uma vez só:

```css
/* src/index.css */
* { box-sizing: border-box; }
body { margin: 0; font-family: sans-serif; }
```

```tsx
// main.tsx
import "./index.css";
```

## 5. Checklist antes de abrir o PR

- [ ] Sem `any` no código
- [ ] Tela mostra carregando, erro e vazio
- [ ] Erro mostrado pro usuário vem da mensagem da API, não `alert()`
- [ ] URL da API vem de variável de ambiente, nunca escrita fixa
- [ ] Arquivo não passou de ~150 linhas (se passou, dá pra quebrar em componentes menores)
- [ ] Sem `console.log` esquecido, sem código comentado
- [ ] Se mudou algo importante na estrutura, escreveu uma linha no README explicando por quê

## 6. Backlog — o que dá pra fazer, em ordem

Cada linha é uma tarefa pequena, um PR só. **P** = rápido, **M** = meio período, **G** = um dia inteiro.

### Bloco 1 — Base (fazer primeiro, os outros dependem disso)

| # | Tarefa | Tamanho |
|---|---|---|
| 1.1 | Criar `src/index.css` bem simples e importar em `main.tsx` | P |
| 1.2 | Completar os tipos de `Task` e `Category` em `types/` | P |
| 1.3 | Completar `api/tasks.ts` com editar, mudar status e excluir | M |
| 1.4 | Criar `api/categories.ts` (listar, criar, editar, excluir) | M |

### Bloco 2 — Separar os componentes que já existem

| # | Tarefa | Tamanho |
|---|---|---|
| 2.1 | Tirar `LoadingState` de dentro de `TaskListPage` | P |
| 2.2 | Tirar `ErrorState` de dentro de `TaskListPage` | P |
| 2.3 | Tirar `EmptyState` de dentro de `TaskListPage` | P |
| 2.4 | Criar `StatusBadge` (uma etiqueta colorida pro status da tarefa) | P |
| 2.5 | Tirar os filtros pra um componente `TaskFilters` | M |
| 2.6 | Tirar o item da lista pra um componente `TaskCard` | M |

### Bloco 3 — Coisas novas de tarefa

| # | Tarefa | Tamanho |
|---|---|---|
| 3.1 | Formulário de criar tarefa (o `createTaskModal.ts` hoje está vazio) | G |
| 3.2 | Usar o mesmo formulário pra editar tarefa | M |
| 3.3 | Botão de concluir/reabrir tarefa | M |
| 3.4 | Botão de excluir, pedindo confirmação antes | P |
| 3.5 | Paginação da lista (próxima página / página anterior) | M |

### Bloco 4 — Categorias (pode fazer ao mesmo tempo que o Bloco 3)

| # | Tarefa | Tamanho |
|---|---|---|
| 4.1 | Tela de listar categorias | M |
| 4.2 | Formulário de criar/editar categoria (com campo de cor) | M |
| 4.3 | Mostrar erro quando o nome da categoria já existe | P |

### Bloco 5 — Painel de resumo

| # | Tarefa | Tamanho |
|---|---|---|
| 5.1 | Função de API que busca o resumo (total, pendentes, concluídas...) | P |
| 5.2 | Card mostrando esses números no topo da lista | M |

### Como dividir entre a galera

- Alguém faz o **Bloco 1** primeiro, sozinho — os outros esperam esse PR ser aprovado.
- Depois disso, **Bloco 2 e Bloco 5** podem rodar ao mesmo tempo, e **Bloco 3 e Bloco 4** também, com pessoas diferentes.
- Cada tarefa = um branch, um PR pequeno. Nada de PR gigante juntando várias tarefas.

## 7. Onde procurar exemplo quando travar

Só o essencial, direto na fonte oficial:

- **React** (como escrever componente, estado, etc): https://react.dev/learn
- **CSS Modules no Vite** (o `.module.css` que usamos): https://vite.dev/guide/features.html#css-modules
- **Variável de ambiente no Vite** (`VITE_API_URL`): https://vite.dev/guide/env-and-mode.html
- **Fetch (como chamar a API)**: https://developer.mozilla.org/pt-BR/docs/Web/API/Fetch_API/Using_Fetch
- **`input type="color"`** (pro campo de cor da categoria): https://developer.mozilla.org/pt-BR/docs/Web/HTML/Element/input/color
- **`<dialog>`** (elemento pronto do HTML pra fazer modal, sem precisar de lib): https://developer.mozilla.org/pt-BR/docs/Web/HTML/Element/dialog

### Exemplo: esperar a pessoa parar de digitar antes de buscar

O desafio pede pra busca por texto não disparar uma requisição a cada letra digitada. Um jeito simples de fazer isso:

```ts
// hooks/useDebouncedValue.ts
import { useEffect, useState } from "react";

export function useDebouncedValue(value: string, delayMs = 400) {
  const [debounced, setDebounced] = useState(value);

  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delayMs);
    return () => clearTimeout(timer);
  }, [value, delayMs]);

  return debounced;
}
```

Uso: espera 400ms depois que a pessoa parou de digitar, só aí busca:

```ts
const debouncedSearch = useDebouncedValue(search);
```

### Exemplo: pegar a mensagem de erro que a API manda

```ts
export async function createTask(data: CreateTaskData) {
  const response = await fetch(`${API_URL}/api/tasks`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const erro = await response.json();
    throw new Error(erro.title ?? "Erro ao criar tarefa");
  }
}
```

## 8. Aprendendo CSS — trilha e onde estudar

Como ninguém no projeto ainda escreveu CSS, segue uma ordem de estudo. Não precisa saber tudo antes de começar — dá pra ir aprendendo item por item conforme for usando no Bloco 1/2.

### Passo 1 — O básico (fazer antes de tudo)
O que aprender: como escrever uma regra de CSS, seletor de classe, cor, texto, espaçamento (`margin`/`padding`).
- **MDN — Primeiros passos em CSS** (em português): https://developer.mozilla.org/pt-BR/docs/Learn/CSS/First_steps
- **freeCodeCamp — Responsive Web Design** (curso gratuito com exercícios, tem certificado): https://www.freecodecamp.org/learn/2022/responsive-web-design/

### Passo 2 — Caixas e organização (o que mais vamos usar aqui)
O que aprender: `box-sizing`, como os elementos ficam um do lado do outro (**Flexbox**) — é o que resolve a barra de filtros e a lista de tarefas.
- **MDN — Box model**: https://developer.mozilla.org/pt-BR/docs/Learn/CSS/Building_blocks/The_box_model
- **Flexbox Froggy** (joguinho pra aprender Flexbox brincando): https://flexboxfroggy.com/#pt-br
- **CSS Tricks — Guia completo de Flexbox** (ótimo pra consultar depois, é tipo um dicionário): https://css-tricks.com/snippets/css/a-guide-to-flexbox/

### Passo 3 — Variáveis e tema (usado no `index.css` do projeto)
O que aprender: `:root { --cor: valor }` e como reaproveitar cor/espaçamento em vários lugares.
- **MDN — Variáveis CSS (custom properties)**: https://developer.mozilla.org/pt-BR/docs/Web/CSS/Using_CSS_custom_properties

### Passo 4 — CSS Modules (a forma que decidimos usar no projeto)
O que aprender: por que cada componente tem seu próprio arquivo `.module.css` e como isso evita um estilo "vazar" pra outro componente sem querer.
- **Documentação do Vite sobre CSS Modules**: https://vite.dev/guide/features.html#css-modules

### Dica de prática
O jeito mais rápido de aprender é abrir o **DevTools do navegador** (F12 → aba "Elements"/"Inspetor") em qualquer site, clicar num elemento e ver o CSS aplicado ali, mudando valores ao vivo pra ver o que acontece. Funciona em qualquer site, é de graça e é o jeito que todo dev usa no dia a dia — não só estudando.
