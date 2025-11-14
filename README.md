# Projeto Tasks -- Full Stack (Backend + Frontend)

Este projeto é uma aplicação full stack composta por:

-   **Backend**: API REST em **ASP.NET Core (.NET 9)** usando Entity
    Framework Core e SQLite.
-   **Frontend**: Aplicação web simples em **HTML, CSS e JavaScript**,
    consumindo a API.

------------------------------------------------------------------------

## 📂 Estrutura do Projeto

BrGaap/
│
├── backend/                # API .NET (TasksAPI)
│   ├── Controllers/
│   ├── Data/
│   ├── Migrations/
│   ├── Models/
│   ├── Properties/
│   ├── appsettings.json
│   │── Program.cs
│   ├── TasksAPI.csproj
│   └── README.md
│
├── frontend/               # Aplicação web
│   └── basic/
│       ├── app.js
│       ├── detail.html
│       ├── detail.js
│       ├── index.html
│       ├── styles.css
│       └── README.md
│
├── TasksAPI.Tests/         # Testes de integração
│   ├── CustomWebApplicationFactory.cs
│   ├── TodosIntegrationTests.cs
│   ├── TasksAPI.Tests.csproj
│   └── README.md
│
├── README.md               # Documentação geral do projeto
└── TasksAPI.sln            # Solução .NET


------------------------------------------------------------------------

## 🚀 Backend -- API Tasks (.NET 9)

### ✔ Tecnologias usadas

-   ASP.NET Core 9
-   Entity Framework Core
-   SQLite
-   Swagger (OpenAPI)

### ✔ URL Base da API

    http://localhost:5188

### ✔ Como rodar o backend

1.  Entre na pasta:

    ``` bash
    cd backend
    ```

2.  Restaurar pacotes:

    ``` bash
    dotnet restore
    ```

3.  Rodar a API:

    ``` bash
    dotnet run
    ```

4.  Endpoints importantes:

    -   Swagger → `http://localhost:5188/swagger`
    -   API base → `http://localhost:5188/todos`

------------------------------------------------------------------------

## 🎨 Frontend -- Interface Web

O frontend está localizado em:

    /frontend/basic

### ✔ Porta usada

Frontend roda via servidor local (ex: VSCode Live Server) na porta:

    http://localhost:5500

### ✔ Como rodar o frontend

Basta abrir o arquivo:

    index.html

Ou usar Live Server / qualquer servidor estático.

------------------------------------------------------------------------

## 🔗 Comunicação entre Front e Back

O frontend faz chamadas para a API usando `fetch()`:

``` js
fetch("http://localhost:5188/todos")
    .then(res => res.json())
    .then(data => renderTasks(data));
```

------------------------------------------------------------------------

## 🗄 Banco de Dados

O projeto utiliza SQLite via Entity Framework Core.

Arquivo:

    backend/tasks.db

------------------------------------------------------------------------

## 🛠 Melhorias Futuras

-   Deploy com Docker
-   Autenticação JWT
-   Frontend refeito em React ou Vue

