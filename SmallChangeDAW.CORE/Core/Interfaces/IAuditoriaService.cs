using SmallChangeDAW.CORE.Core.DTOs;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface IAuditoriaService
{
    /// <summary>
    /// Registra una auditoría de creación de transacción
    /// </summary>
    Task RegistrarCreacionAsync(int transaccionId, int usuarioId, string estadoNuevo);

    /// <summary>
    /// Registra una auditoría de cambio de estado de transacción
    /// </summary>
    Task RegistrarCambioEstadoAsync(int transaccionId, int usuarioId, string estadoAnterior, string estadoNuevo);

    /// <summary>
    /// Registra una auditoría de eliminación de transacción
    /// </summary>
    Task RegistrarEliminacionAsync(int transaccionId, int usuarioId, string estadoAnterior);

    /// <summary>
    /// Obtiene el historial de auditoría de una transacción
    /// </summary>
    Task<IEnumerable<AuditoriaTransaccionResponseDTO>> ObtenerHistorialAsync(int transaccionId);

    /// <summary>
    /// Obtiene todas las auditorías del usuario autenticado
    /// </summary>
    Task<IEnumerable<AuditoriaTransaccionResponseDTO>> ObtenerAuditoriasPorUsuarioAsync(int usuarioId);
}
