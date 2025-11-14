# Backend - TasksAPI

Este projeto é uma API RESTful desenvolvida em .NET para gerenciamento de tarefas (todos). Ele utiliza Entity Framework Core para persistência de dados e segue boas práticas de arquitetura para aplicações web.

## Estrutura de Pastas
- **Controllers/**: Contém os controladores da API, como `TodosController.cs`.
- **Data/**: Inclui o contexto do banco de dados (`AppDbContext.cs`).
- **Migrations/**: Arquivos de migração do Entity Framework para versionamento do banco de dados.
- **Models/**: Modelos de dados `Todo.cs`.
- **Properties/**: Configurações de inicialização, como `launchSettings.json`.
- **appsettings.json / appsettings.Development.json**: Arquivos de configuração da aplicação.
- **Program.cs**: Ponto de entrada da aplicação.
- **TasksAPI.csproj**: Arquivo de configuração do projeto .NET.

## Principais Funcionalidades
- CRUD de tarefas (todos)
- Persistência com Entity Framework Core
- Suporte a migrações de banco de dados
- Configuração para ambientes de desenvolvimento e produção

## Como Executar
1. Instale o .NET SDK 9.0 ou superior.
2. No terminal, navegue até a pasta `backend`:
   ```powershell
   cd backend
   ```
3. Execute o comando para iniciar a API:
   ```powershell
   dotnet run
   ```
4. A API estará disponível em `https://localhost:5188`.

## Testes
Os testes de integração estão localizados em `../TasksAPI.Tests`. Para rodar os testes:
```powershell
cd ../TasksAPI.Tests
 dotnet test
```

## Dependências Principais
- ASP.NET Core
- Entity Framework Core

## Migrações
Para criar uma nova migração:
```powershell
dotnet ef migrations add NomeDaMigracao
```
Para atualizar o banco de dados:
```powershell
dotnet ef database update
```
