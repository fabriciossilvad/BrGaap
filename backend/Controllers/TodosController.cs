using TasksAPI.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksAPI.Models;

namespace TasksAPI.Controllers
{
    [ApiController]
    [Route("todos")]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public TodosController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // DTOs
        public class UpdateCompletedDto
        {
            public bool Completed { get; set; }
        }
        public class CreateTodoDto
        {
            public required string Title { get; set; }
            public required int UserId { get; set; }
            public bool Completed { get; set; }
        }

        // GET /todos
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? title,
            [FromQuery] int? userId,
            [FromQuery] bool? completed,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "asc")
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<Todo> query = _context.Todos.AsNoTracking();

            // 🔍 Filtragem
            if (!string.IsNullOrEmpty(title))
            {
                var t = title.Trim();
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{t}%"));
            }

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }

            if (completed.HasValue)
            {
                query = query.Where(x => x.Completed == completed.Value);
            }

            // 🔢 Ordenação dinâmica
            bool ascending = string.Equals(order, "asc", StringComparison.OrdinalIgnoreCase);
            query = sort?.ToLower() switch
            {
                "title" => ascending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                "completed" => ascending ? query.OrderBy(t => t.Completed) : query.OrderByDescending(t => t.Completed),
                "userid" or "userId" => ascending ? query.OrderBy(t => t.UserId) : query.OrderByDescending(t => t.UserId),
                _ => ascending ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Id),
            };

            // 📊 Paginação
            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                page,
                pageSize,
                total,
                items
            };

            return Ok(result);
        }

        // GET /todos/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var todo = await _context.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        // PUT /todos/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompleted([FromRoute] int id, [FromBody] UpdateCompletedDto dto)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            if (todo.Completed == dto.Completed)
            {
                return NoContent();
            }

            // Regra de Negócio: Um usuário não pode ter mais de 5 tarefas incompletas.
            if (!dto.Completed)
            {
                var incompleteCount = await _context.Todos
                    .CountAsync(t => t.UserId == todo.UserId && !t.Completed && t.Id != todo.Id);

                if (incompleteCount >= 5)
                {
                    return BadRequest("O usuário já possui 5 tarefas incompletas.");
                }
            }

            todo.Completed = dto.Completed;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST /todos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Título é obrigatório.");

            var todo = new Todo
            {
                Title = dto.Title.Trim(),
                UserId = dto.UserId,
                Completed = dto.Completed
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return Created($"/todos/{todo.Id}", todo);
        }

        // POST /todos/sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncFromExternal()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");

            List<Todo>? externalTodos;
            try
            {
                externalTodos = await client.GetFromJsonAsync<List<Todo>>("todos");
            }
            catch (Exception ex)
            {
                return StatusCode(502, new { message = "Erro ao acessar o serviço externo.", detail = ex.Message });
            }

            if (externalTodos == null || externalTodos.Count == 0)
            {
                return BadRequest(new { message = "Nenhum dado recebido da fonte externa." });
            }

            foreach (var ext in externalTodos)
            {
                var existing = await _context.Todos.FirstOrDefaultAsync(t => t.Id == ext.Id);
                if (existing == null)
                {
                    _context.Todos.Add(new Todo
                    {
                        Id = ext.Id,
                        Title = ext.Title,
                        Completed = ext.Completed,
                        UserId = ext.UserId
                    });
                }
                else
                {
                    existing.Title = ext.Title;
                    existing.Completed = ext.Completed;
                    existing.UserId = ext.UserId;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { imported = externalTodos.Count });
        }
    }
}
