namespace insightflow_workspace_service.Models;

/// <summary>
/// Modelo que representa un espacio de trabajo en InsightFlow
/// </summary>
public class Workspace
{
    /// <summary>
    /// Identificador único del espacio de trabajo (UUID V4)
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre del espacio de trabajo (debe ser único)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del espacio de trabajo
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Temática del espacio de trabajo (ej: Educación, Trabajo, Personal)
    /// </summary>
    public string Theme { get; set; } = string.Empty;

    /// <summary>
    /// URL del ícono del espacio de trabajo
    /// </summary>
    public string IconUrl { get; set; } = string.Empty;

    /// <summary>
    /// ID del usuario propietario del espacio
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Indica si el espacio está activo (para SOFT DELETE)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de creación del espacio
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de última actualización del espacio
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Fecha de eliminación (SOFT DELETE)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Lista de miembros del espacio de trabajo
    /// </summary>
    public List<WorkspaceMember> Members { get; set; } = new();
}