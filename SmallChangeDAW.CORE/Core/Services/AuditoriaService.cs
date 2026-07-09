using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IClientesRepository _clientesRepository;

    public AuditoriaService(IAuditoriaRepository auditoriaRepository, IClientesRepository clientesRepository)
    {
        _auditoriaRepository = auditoriaRepository;
        _clientesRepository = clientesRepository;
    }

    public async Task RegistrarCreacionAsync(int transaccionId, int usuarioId, string estadoNuevo)
    {
        var auditoria = new AuditoriaTransaccion
        {
            transaccion_id = transaccionId,
            usuario_id = usuarioId,
            accion = "CREAR",
            estado_anterior = null,
            estado_nuevo = estadoNuevo,
            fecha_accion = DateTime.UtcNow
        };

        await _auditoriaRepository.AddAsync(auditoria);
    }

    public async Task RegistrarCambioEstadoAsync(int transaccionId, int usuarioId, string estadoAnterior, string estadoNuevo)
    {
        var auditoria = new AuditoriaTransaccion
        {
            transaccion_id = transaccionId,
            usuario_id = usuarioId,
            accion = "ACTUALIZAR_ESTADO",
            estado_anterior = estadoAnterior,
            estado_nuevo = estadoNuevo,
            fecha_accion = DateTime.UtcNow
        };

        await _auditoriaRepository.AddAsync(auditoria);
    }

    public async Task RegistrarEliminacionAsync(int transaccionId, int usuarioId, string estadoAnterior)
    {
        var auditoria = new AuditoriaTransaccion
        {
            transaccion_id = transaccionId,
            usuario_id = usuarioId,
            accion = "ELIMINAR",
            estado_anterior = estadoAnterior,
            estado_nuevo = "eliminada",
            fecha_accion = DateTime.UtcNow
        };

        await _auditoriaRepository.AddAsync(auditoria);
    }

    public async Task<IEnumerable<AuditoriaTransaccionResponseDTO>> ObtenerHistorialAsync(int transaccionId)
    {
        var auditorias = await _auditoriaRepository.GetByTransaccionIdAsync(transaccionId);
        return auditorias.Select(MapToDTO);
    }

    public async Task<IEnumerable<AuditoriaTransaccionResponseDTO>> ObtenerAuditoriasPorUsuarioAsync(int usuarioId)
    {
        var auditorias = await _auditoriaRepository.GetByUsuarioIdAsync(usuarioId);
        return auditorias.Select(MapToDTO);
    }

    private static AuditoriaTransaccionResponseDTO MapToDTO(AuditoriaTransaccion auditoria)
    {
        return new AuditoriaTransaccionResponseDTO
        {
            Id = auditoria.id,
            TransaccionId = auditoria.transaccion_id,
            UsuarioId = auditoria.usuario_id,
            Accion = auditoria.accion,
            EstadoAnterior = auditoria.estado_anterior,
            EstadoNuevo = auditoria.estado_nuevo,
            FechaAccion = auditoria.fecha_accion,
            Usuario = auditoria.Usuario != null ? new ClienteAuditoriaDTO
            {
                Id = auditoria.Usuario.id,
                Nombre = auditoria.Usuario.nombre,
                Email = auditoria.Usuario.email
            } : null
        };
    }
}
