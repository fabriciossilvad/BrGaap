# Frontend - BrGaap

Este frontend é uma aplicação web simples para gerenciamento de tarefas (todos), conectando-se à API backend. Ele está localizado na pasta `frontend/basic` e utiliza HTML, CSS e JavaScript puro.

## Estrutura de Pastas
- **index.html**: Página principal, exibe a lista de tarefas.
- **detail.html**: Página de detalhes de uma tarefa específica.
- **app.js**: Script principal, responsável por interações e requisições à API.
- **detail.js**: Script para manipulação da página de detalhes.
- **styles.css**: Estilos visuais da aplicação.

## Funcionalidades
- Listagem de tarefas
- Visualização de detalhes de uma tarefa
- Adição e edição de tarefas (via integração com backend).
- Importação de tarefas de `https://jsonplaceholder.typicode.com/todos`
- Interface responsiva e simples

## Como Executar
1. Certifique-se que o backend está rodando (veja instruções no README do backend).
2. Execute o arquivo `index.html` na porta 5500.
3. Para visualizar detalhes de uma tarefa, clique sobre ela na lista.

## Integração
- O frontend faz requisições para a API backend (por padrão, em `http://localhost:5188`).
- Certifique-se de ajustar URLs no `app.js` e `detail.js` caso o backend esteja em outra porta ou endereço.

## Requisitos
- Backend ativo para leitura e persistência dos dados
- Frontend deve estar rodando em `http://localhost:5500` (Porta padrão caso utilize o Live Server)
