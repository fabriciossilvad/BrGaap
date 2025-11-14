using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TasksAPI.Models;
using Xunit;

public class TodosIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TodosIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ---------------------------
    // GET /todos
    // ----------------------------

    [Fact]
    public async Task GetAll_DeveRetornarStatus200()
    {
        var response = await _client.GetAsync("/todos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.True(json.GetProperty("page").GetInt32() >= 1);
        Assert.True(json.GetProperty("items").GetArrayLength() >= 0);
    }

    [Fact]
    public async Task GetAll_ComFiltroTitle_DeveRetornarSomenteItensFiltrados()
    {
        var response = await _client.GetAsync("/todos?title=ESTUDAR");

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var items = json.GetProperty("items").EnumerateArray();

        foreach (var item in items)
        {
            string title = item.GetProperty("title").GetString()!;
            Assert.Contains("ESTUDAR", title, StringComparison.OrdinalIgnoreCase);
        }
    }

    // ---------------------------
    // GET /todos/{id}
    // ----------------------------

    [Fact]
    public async Task GetById_DeveRetornarItem()
    {
        var createdResponse = await _client.PostAsJsonAsync("/todos", new
        {
            title = "GET BY ID",
            userId = 10,
            completed = true
        });

        var created = await createdResponse.Content.ReadFromJsonAsync<JsonElement>();
        int id = created.GetProperty("id").GetInt32();

        var response = await _client.GetAsync($"/todos/{id}");
        response.EnsureSuccessStatusCode();

        var todo = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal("GET BY ID", todo.GetProperty("title").GetString());
    }

    // ---------------------------
    // POST /todos
    // ----------------------------

    [Fact]
    public async Task Create_DeveCriarTodo()
    {
        var newTodo = new
        {
            title = "Teste integração POST",
            userId = 99,
            completed = true
        };

        var response = await _client.PostAsJsonAsync("/todos", newTodo);

        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal("Teste integração POST", created.GetProperty("title").GetString());
        Assert.Equal(99, created.GetProperty("userId").GetInt32());
    }

    [Fact]
    public async Task Create_NaoDevePermitirMaisDe5Incompletas()
    {
        int userId = 50;

        for (int i = 0; i < 5; i++)
        {
            await _client.PostAsJsonAsync("/todos", new
            {
                title = $"Task{i}",
                userId,
                completed = false
            });
        }

        var response = await _client.PostAsJsonAsync("/todos", new
        {
            title = "Task extra",
            userId,
            completed = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------------------------
    // PUT /todos/{id}
    // ----------------------------

    [Fact]
    public async Task UpdateCompleted_DeveAtualizarStatus()
    {
        var createResponse = await _client.PostAsJsonAsync("/todos", new
        {
            title = "PUT Test",
            userId = 44,
            completed = false
        });

        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        int id = created.GetProperty("id").GetInt32();

        var response = await _client.PutAsJsonAsync($"/todos/{id}", new { completed = true });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCompleted_NaoPermiteVoltarParaIncompletaSeLimiteAtingido()
    {
        int userId = 70;

        // Cria 5 tarefas incompletas
        for (int i = 1; i <= 5; i++)
        {
            await _client.PostAsJsonAsync("/todos", new
            {
                title = $"T{i}",
                userId,
                completed = false
            });
        }

        // Cria uma COMPLETA
        var createdResponse = await _client.PostAsJsonAsync("/todos", new
        {
            title = "Completinha",
            userId,
            completed = true
        });

        var created = await createdResponse.Content.ReadFromJsonAsync<JsonElement>();
        int id = created.GetProperty("id").GetInt32();

        // Tenta voltar para incompleta → deve falhar
        var response = await _client.PutAsJsonAsync($"/todos/{id}", new { completed = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
