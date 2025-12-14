namespace insightflow_workspace_service.Models;

/// <summary>
/// Modelo que representa un miembro de un espacio de trabajo
/// </summary>
public class WorkspaceMember
{
    /// <summary>
    /// ID del usuario miembro
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario en el espacio (Propietario, Editor)
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Fecha en que el usuario se unió al espacio
    /// </summary>
    public DateTime JoinedAt { get; set; }
}