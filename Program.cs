using insightflow_workspace_service.Endpoints;
using insightflow_workspace_service.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURACIÓN DE SERVICIOS ====================

// Agregar servicios de documentación API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "InsightFlow - Workspace Service API",
        Version = "v1",
        Description = "API para gestión de espacios de trabajo colaborativos",
        Contact = new()
        {
            Name = "Equipo InsightFlow",
            Email = "contacto@insightflow.com"
        }
    });
});

// Configurar CORS para permitir peticiones desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // Permitir cualquier origen
              .AllowAnyMethod()      // Permitir cualquier método HTTP
              .AllowAnyHeader();     // Permitir cualquier header
    });
});

// Registrar el repositorio como Singleton (datos en memoria)
// Singleton: una única instancia durante toda la vida de la aplicación
builder.Services.AddSingleton<WorkspaceRepository>();

var app = builder.Build();

// ==================== CONFIGURACIÓN DEL PIPELINE HTTP ====================

// Habilitar Swagger en desarrollo y producción
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Workspace Service API v1");
        options.RoutePrefix = "swagger";
    });
}

// Habilitar CORS antes de los endpoints
app.UseCors("AllowAll");

// ==================== INICIALIZACIÓN DE DATOS ====================

// Obtener el repositorio e inicializar con datos de ejemplo
var repository = app.Services.GetRequiredService<WorkspaceRepository>();
repository.SeedData();

// ==================== MAPEO DE ENDPOINTS ====================

// Mapear todos los endpoints de workspace
app.MapWorkspaceEndpoints();

// Endpoint raíz de bienvenida
app.MapGet("/", () => new
{
    service = "InsightFlow - Workspace Service",
    version = "1.0.0",
    status = "running",
    documentation = "/swagger",
    endpoints = new[]
    {
        "POST /workspaces",
        "GET /workspaces",
        "GET /workspaces/{id}",
        "PATCH /workspaces/{id}",
        "DELETE /workspaces/{id}"
    }
})
.WithName("Root")
.WithTags("General")
.WithSummary("Información del servicio")
.ExcludeFromDescription();

// Endpoint de health check
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    uptime = Environment.TickCount64 / 1000 // segundos
})
.WithName("HealthCheck")
.WithTags("General")
.WithSummary("Verificar estado del servicio")
.ExcludeFromDescription();

// ==================== INICIAR LA APLICACIÓN ====================

app.Run();

// Hacer la clase Program parcial para testing (opcional)
public partial class Program { }