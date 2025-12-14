using insightflow_workspace_service.Models;

namespace insightflow_workspace_service.Repositories;

/// <summary>
/// Repositorio en memoria para gestionar espacios de trabajo
/// Implementa el patrón Repository para abstraer el acceso a datos
/// </summary>
public class WorkspaceRepository
{
    // Lista en memoria que simula una base de datos
    private readonly List<Workspace> _workspaces = new();

    /// <summary>
    /// Inicializa el repositorio con datos de ejemplo (Seeder)
    /// </summary>
    public void SeedData()
    {
        // IDs de usuarios de ejemplo
        var userId1 = Guid.Parse("550e8400-e29b-41d4-a716-446655440001");
        var userId2 = Guid.Parse("550e8400-e29b-41d4-a716-446655440002");

        // Agregar espacios de trabajo de ejemplo
        _workspaces.AddRange(new[]
        {
            // Workspace 1: Proyecto Universidad
            new Workspace
            {
                Id = Guid.NewGuid(),
                Name = "Proyecto Universidad",
                Description = "Espacio para trabajos académicos y colaboración estudiantil",
                Theme = "Educación",
                IconUrl = "https://via.placeholder.com/150/0000FF/808080",
                OwnerId = userId1,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                IsActive = true,
                Members = new List<WorkspaceMember>
                {
                    new() 
                    { 
                        UserId = userId1, 
                        UserName = "Juan Pérez", 
                        Role = "Propietario", 
                        JoinedAt = DateTime.UtcNow.AddDays(-30) 
                    },
                    new() 
                    { 
                        UserId = userId2, 
                        UserName = "María González", 
                        Role = "Editor", 
                        JoinedAt = DateTime.UtcNow.AddDays(-25) 
                    }
                }
            },

            // Workspace 2: Ideas Personales
            new Workspace
            {
                Id = Guid.NewGuid(),
                Name = "Ideas Personales",
                Description = "Notas y reflexiones personales sobre diversos temas",
                Theme = "Personal",
                IconUrl = "https://via.placeholder.com/150/FF0000/FFFFFF",
                OwnerId = userId1,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                IsActive = true,
                Members = new List<WorkspaceMember>
                {
                    new() 
                    { 
                        UserId = userId1, 
                        UserName = "Juan Pérez", 
                        Role = "Propietario", 
                        JoinedAt = DateTime.UtcNow.AddDays(-15) 
                    }
                }
            },

            // Workspace 3: Desarrollo Web
            new Workspace
            {
                Id = Guid.NewGuid(),
                Name = "Desarrollo Web",
                Description = "Recursos, guías y tutoriales de desarrollo web moderno",
                Theme = "Tecnología",
                IconUrl = "https://via.placeholder.com/150/00FF00/000000",
                OwnerId = userId2,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                IsActive = true,
                Members = new List<WorkspaceMember>
                {
                    new() 
                    { 
                        UserId = userId2, 
                        UserName = "María González", 
                        Role = "Propietario", 
                        JoinedAt = DateTime.UtcNow.AddDays(-20) 
                    },
                    new() 
                    { 
                        UserId = userId1, 
                        UserName = "Juan Pérez", 
                        Role = "Editor", 
                        JoinedAt = DateTime.UtcNow.AddDays(-18) 
                    }
                }
            }
        });
    }

    /// <summary>
    /// Agrega un nuevo workspace al repositorio
    /// </summary>
    /// <param name="workspace">Workspace a agregar</param>
    public void Add(Workspace workspace)
    {
        _workspaces.Add(workspace);
    }

    /// <summary>
    /// Obtiene un workspace por su ID (solo si está activo)
    /// </summary>
    /// <param name="id">ID del workspace</param>
    /// <returns>Workspace encontrado o null</returns>
    public Workspace? GetById(Guid id)
    {
        return _workspaces.FirstOrDefault(w => w.Id == id && w.IsActive);
    }

    /// <summary>
    /// Obtiene todos los workspaces donde el usuario es miembro
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de workspaces del usuario</returns>
    public IEnumerable<Workspace> GetByUserId(Guid userId)
    {
        return _workspaces
            .Where(w => w.IsActive && w.Members.Any(m => m.UserId == userId))
            .OrderByDescending(w => w.CreatedAt);
    }

    /// <summary>
    /// Verifica si existe un workspace con el nombre dado (activo)
    /// </summary>
    /// <param name="name">Nombre a verificar</param>
    /// <returns>True si existe, False si no</returns>
    public bool ExistsByName(string name)
    {
        return _workspaces.Any(w => 
            w.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
            w.IsActive);
    }

    /// <summary>
    /// Obtiene todos los workspaces activos (para propósitos administrativos)
    /// </summary>
    /// <returns>Lista de todos los workspaces activos</returns>
    public IEnumerable<Workspace> GetAll()
    {
        return _workspaces.Where(w => w.IsActive).OrderByDescending(w => w.CreatedAt);
    }

    /// <summary>
    /// Obtiene la cantidad total de workspaces activos
    /// </summary>
    /// <returns>Cantidad de workspaces</returns>
    public int Count()
    {
        return _workspaces.Count(w => w.IsActive);
    }
}