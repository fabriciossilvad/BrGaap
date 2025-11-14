using Microsoft.EntityFrameworkCore;
using TasksAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurações de Serviço.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Configuração do banco de dados
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Limitar conexão apenas ao frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOnlyLocalFrontend", policy =>
        policy
            .WithOrigins("http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();
// Configuração do Pipeline HTTP.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TasksAPI v1");
        options.DocumentTitle = "TasksAPI - Documentação da API";

        // Remove o logo Swagger, o seletor de versão e ajusta o topo
        options.HeadContent = @"
            <style>
            /* ----- CUSTOMIZAÇÃO SWAGGER UI COMPLETA ----- */

            /* Cor da barra superior */
            .swagger-ui .topbar, .topbar {
                background-color: #0A84FF !important;
            }

            /* Remove completamente o conteúdo da topbar (logo, link e textos) */
            .swagger-ui .topbar-wrapper,
            .topbar-wrapper,
            .swagger-ui .topbar a,
            .swagger-ui .topbar a:after,
            .swagger-ui .topbar .link,
            .swagger-ui .topbar .link span,
            .swagger-ui .topbar .link img,
            .swagger-ui .topbar .link:after,
            .swagger-ui .topbar .topbar-wrapper,
            .swagger-ui .topbar .topbar-wrapper a,
            .swagger-ui .topbar .topbar-wrapper span,
            .swagger-ui .topbar .topbar-wrapper img,
            .swagger-ui .topbar .topbar-wrapper:after,
            .swagger-ui .topbar .download-url-wrapper,
            .swagger-ui .topbar .download-url-wrapper *,
            .swagger-ui .topbar a[href*='swagger.io'],
            .swagger-ui .topbar a[href*='smartbear'],
            .swagger-ui .topbar a[href*='api'],
            .swagger-ui .topbar small,
            .swagger-ui .topbar span,
            .topbar-wrapper [href*='swagger.io'],
            .topbar-wrapper [href*='smartbear'] {
                display: none !important;
            }

            /* Título da API */
            .swagger-ui .info .title,
            .swagger-ui .title {
                font-family: 'Segoe UI', Arial, sans-serif !important;
                color: #0A84FF !important;
            }

            /* Fundo e fonte geral */
            body {
                font-family: 'Segoe UI', Arial, sans-serif !important;
                background-color: #f9f9f9 !important;
            }
            </style>
            ";



    });
}


if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowOnlyLocalFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();

// Visibilidade interna para testes
public partial class Program { }
