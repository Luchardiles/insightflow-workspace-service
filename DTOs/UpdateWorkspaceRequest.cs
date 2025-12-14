using System.ComponentModel.DataAnnotations;

namespace insightflow_workspace_service.DTOs;

/// <summary>
/// DTO para la solicitud de actualización de un espacio de trabajo
/// </summary>
public class UpdateWorkspaceRequest
{
    /// <summary>
    /// ID del usuario que realiza la actualización (debe ser propietario)
    /// </summary>
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Nuevo nombre del espacio (opcional)
    /// </summary>
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string? Name { get; set; }

    /// <summary>
    /// Nueva URL del ícono (opcional)
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Nueva descripción (opcional)
    /// </summary>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    /// <summary>
    /// Nueva temática (opcional)
    /// </summary>
    public string? Theme { get; set; }
}