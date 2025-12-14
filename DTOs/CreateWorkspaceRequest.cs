using System.ComponentModel.DataAnnotations;

namespace insightflow_workspace_service.DTOs;

/// <summary>
/// DTO para la solicitud de creación de un espacio de trabajo
/// </summary>
public class CreateWorkspaceRequest
{
    /// <summary>
    /// Nombre del espacio de trabajo
    /// </summary>
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del espacio de trabajo
    /// </summary>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Temática del espacio de trabajo
    /// </summary>
    [Required(ErrorMessage = "La temática es requerida")]
    public string Theme { get; set; } = string.Empty;

    /// <summary>
    /// URL del ícono del espacio
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// ID del usuario que crea el espacio
    /// </summary>
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Nombre del usuario que crea el espacio
    /// </summary>
    public string? UserName { get; set; }
}