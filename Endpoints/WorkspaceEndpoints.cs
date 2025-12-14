using Microsoft.AspNetCore.Mvc;
using insightflow_workspace_service.DTOs;
using insightflow_workspace_service.Models;
using insightflow_workspace_service.Repositories;

namespace insightflow_workspace_service.Endpoints;

/// <summary>
/// Clase estática que define todos los endpoints del Workspace Service
/// </summary>
public static class WorkspaceEndpoints
{
    /// <summary>
    /// Mapea todos los endpoints de workspace a la aplicación
    /// </summary>
    /// <param name="app">Aplicación web</param>
    public static void MapWorkspaceEndpoints(this WebApplication app)
    {
        // Grupo de endpoints para workspaces
        var group = app.MapGroup("/workspaces")
            .WithTags("Workspaces")
            .WithOpenApi();

        // POST /workspaces - Crear espacio de trabajo
        group.MapPost("/", CreateWorkspace)
            .WithName("CreateWorkspace")
            .WithSummary("Crear un nuevo espacio de trabajo")
            .WithDescription("Crea un nuevo espacio de trabajo asignando un UUID único y registrando al creador como propietario");

        // GET /workspaces - Listar espacios por usuario
        group.MapGet("/", GetWorkspacesByUser)
            .WithName("GetWorkspaces")
            .WithSummary("Listar espacios de trabajo de un usuario")
            .WithDescription("Obtiene todos los espacios donde el usuario es miembro");

        // GET /workspaces/{id} - Obtener espacio por ID
        group.MapGet("/{id:guid}", GetWorkspaceById)
            .WithName("GetWorkspaceById")
            .WithSummary("Obtener espacio de trabajo por ID")
            .WithDescription("Obtiene información detallada de un espacio específico incluyendo sus miembros");

        // PATCH /workspaces/{id} - Actualizar espacio
        group.MapPatch("/{id:guid}", UpdateWorkspace)
            .WithName("UpdateWorkspace")
            .WithSummary("Actualizar espacio de trabajo")
            .WithDescription("Actualiza los datos de un espacio (solo propietarios)");

        // DELETE /workspaces/{id} - Eliminar espacio (SOFT DELETE)
        group.MapDelete("/{id:guid}", DeleteWorkspace)
            .WithName("DeleteWorkspace")
            .WithSummary("Eliminar espacio de trabajo")
            .WithDescription("Desactiva un espacio mediante SOFT DELETE (solo propietarios)");
    }

    /// <summary>
    /// Endpoint para crear un nuevo espacio de trabajo
    /// </summary>
    private static IResult CreateWorkspace(
        [FromBody] CreateWorkspaceRequest request,
        WorkspaceRepository repo)
    {
        // Validar que el nombre sea único
        if (repo.ExistsByName(request.Name))
        {
            return Results.BadRequest(new 
            { 
                error = "Ya existe un espacio de trabajo con ese nombre" 
            });
        }

        // Crear nuevo workspace con UUID único
        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Theme = request.Theme,
            IconUrl = request.IconUrl ?? "https://via.placeholder.com/150",
            OwnerId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Agregar al propietario como miembro con rol "Propietario"
        workspace.Members.Add(new WorkspaceMember
        {
            UserId = request.UserId,
            UserName = request.UserName ?? "Usuario",
            Role = "Propietario",
            JoinedAt = DateTime.UtcNow
        });

        // Guardar en el repositorio
        repo.Add(workspace);

        // Retornar el workspace creado con status 201 Created
        return Results.Created($"/workspaces/{workspace.Id}", new
        {
            id = workspace.Id,
            name = workspace.Name,
            description = workspace.Description,
            theme = workspace.Theme,
            iconUrl = workspace.IconUrl,
            ownerId = workspace.OwnerId,
            createdAt = workspace.CreatedAt
        });
    }

    /// <summary>
    /// Endpoint para listar espacios de trabajo de un usuario
    /// </summary>
    private static IResult GetWorkspacesByUser(
        [FromQuery] Guid? userId,
        WorkspaceRepository repo)
    {
        // Validar que se proporcione el userId
        if (!userId.HasValue)
        {
            return Results.BadRequest(new 
            { 
                error = "Se requiere el parámetro userId" 
            });
        }

        // Obtener workspaces del usuario
        var userWorkspaces = repo.GetByUserId(userId.Value);

        // Mapear a DTO de respuesta
        var response = userWorkspaces.Select(w => new
        {
            id = w.Id,
            name = w.Name,
            iconUrl = w.IconUrl,
            role = w.Members.FirstOrDefault(m => m.UserId == userId.Value)?.Role ?? "Editor",
            theme = w.Theme,
            createdAt = w.CreatedAt
        });

        return Results.Ok(response);
    }

    /// <summary>
    /// Endpoint para obtener un espacio de trabajo por su ID
    /// </summary>
    private static IResult GetWorkspaceById(
        Guid id,
        [FromQuery] Guid? userId,
        WorkspaceRepository repo)
    {
        // Buscar el workspace
        var workspace = repo.GetById(id);

        if (workspace == null)
        {
            return Results.NotFound(new 
            { 
                error = "Espacio de trabajo no encontrado" 
            });
        }

        // Verificar si el usuario es miembro (opcional según el requerimiento)
        if (userId.HasValue)
        {
            var member = workspace.Members.FirstOrDefault(m => m.UserId == userId.Value);
            if (member == null)
            {
                return Results.Json(
                    new { error = "No tienes acceso a este espacio de trabajo" },
                    statusCode: 403
                );
            }
        }

        // Retornar información completa del workspace
        var response = new
        {
            id = workspace.Id,
            name = workspace.Name,
            description = workspace.Description,
            theme = workspace.Theme,
            iconUrl = workspace.IconUrl,
            ownerId = workspace.OwnerId,
            createdAt = workspace.CreatedAt,
            updatedAt = workspace.UpdatedAt,
            members = workspace.Members.Select(m => new
            {
                userId = m.UserId,
                userName = m.UserName,
                role = m.Role,
                joinedAt = m.JoinedAt
            })
        };

        return Results.Ok(response);
    }

    /// <summary>
    /// Endpoint para actualizar un espacio de trabajo
    /// </summary>
    private static IResult UpdateWorkspace(
        Guid id,
        [FromBody] UpdateWorkspaceRequest request,
        WorkspaceRepository repo)
    {
        // Buscar el workspace
        var workspace = repo.GetById(id);

        if (workspace == null)
        {
            return Results.NotFound(new 
            { 
                error = "Espacio de trabajo no encontrado" 
            });
        }

        // Verificar que el usuario sea propietario
        var member = workspace.Members.FirstOrDefault(m => m.UserId == request.UserId);
        if (member == null || member.Role != "Propietario")
        {
            return Results.Json(
                new { error = "Solo el propietario puede editar este espacio" },
                statusCode: 403
            );
        }

        // Validar nombre único si se está cambiando
        if (!string.IsNullOrEmpty(request.Name) && request.Name != workspace.Name)
        {
            if (repo.ExistsByName(request.Name))
            {
                return Results.BadRequest(new 
                { 
                    error = "Ya existe un espacio de trabajo con ese nombre" 
                });
            }
            workspace.Name = request.Name;
        }

        // Actualizar campos si se proporcionan
        if (!string.IsNullOrEmpty(request.IconUrl))
        {
            workspace.IconUrl = request.IconUrl;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            workspace.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.Theme))
        {
            workspace.Theme = request.Theme;
        }

        // Actualizar timestamp
        workspace.UpdatedAt = DateTime.UtcNow;

        // Retornar workspace actualizado
        return Results.Ok(new
        {
            id = workspace.Id,
            name = workspace.Name,
            description = workspace.Description,
            theme = workspace.Theme,
            iconUrl = workspace.IconUrl,
            updatedAt = workspace.UpdatedAt
        });
    }

    /// <summary>
    /// Endpoint para eliminar (desactivar) un espacio de trabajo
    /// </summary>
    private static IResult DeleteWorkspace(
        Guid id,
        [FromQuery] Guid userId,
        WorkspaceRepository repo)
    {
        // Buscar el workspace
        var workspace = repo.GetById(id);

        if (workspace == null)
        {
            return Results.NotFound(new 
            { 
                error = "Espacio de trabajo no encontrado" 
            });
        }

        // Verificar que el usuario sea propietario
        if (workspace.OwnerId != userId)
        {
            return Results.Json(
                new { error = "Solo el propietario puede eliminar este espacio" },
                statusCode: 403
            );
        }

        // SOFT DELETE: marcar como inactivo
        workspace.IsActive = false;
        workspace.DeletedAt = DateTime.UtcNow;

        return Results.Ok(new 
        { 
            message = "Espacio de trabajo desactivado exitosamente",
            id = workspace.Id,
            deletedAt = workspace.DeletedAt
        });
    }
}