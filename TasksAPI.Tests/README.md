# Testes de Integração - TasksAPI

Esta pasta contém os testes de integração para a API de tarefas (`TasksAPI`). Os testes garantem que os principais endpoints da API funcionam corretamente em cenários reais, utilizando um ambiente de teste isolado.

## Estrutura
- **TodosIntegrationTests.cs**: Testes de integração para os endpoints de tarefas (CRUD).
- **CustomWebApplicationFactory.cs**: Classe utilitária para configurar o ambiente de teste, simulando o servidor da API.
- **TasksAPI.Tests.csproj**: Arquivo de configuração do projeto de testes.

## Como Executar os Testes
1. Certifique-se de que o backend está compilado e que as dependências estão instaladas.
2. No terminal, navegue até a pasta `TasksAPI.Tests`:
   ```powershell
   cd TasksAPI.Tests
   ```
3. Execute os testes:
   ```powershell
   dotnet test
   ```

## O que é testado?
- Criação, leitura, atualização e regra de negócio de tarefas via API
- Respostas e status HTTP dos endpoints
- Integração real com o banco de dados em ambiente de teste

## Observações
- Os testes utilizam um banco de dados em memória para garantir isolamento.
- O arquivo `CustomWebApplicationFactory.cs` permite customizar o ambiente de execução dos testes.

## Requisitos
- .NET SDK 9.0 ou superior
