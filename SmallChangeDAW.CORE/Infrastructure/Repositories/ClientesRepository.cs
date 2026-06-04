using Dapper;
using SmallChangeDAW.Core.Interfaces;
using SmallChangeDAW.Infrastructure.Data;
using SmallChangeDAW.Models;

namespace SmallChangeDAW.Infrastructure.Repositories;

public class ClientesRepository : IClientesRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ClientesRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Cliente>("SELECT * FROM Clientes");
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Cliente>(
            "SELECT * FROM Clientes WHERE id = @Id", new { Id = id });
    }

    public async Task<int> AddAsync(Cliente cliente)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Clientes (nombre, email, pass_hash, promedio_calificacion_comprador, calificacion_vendedor)
                    VALUES (@Nombre, @Email, @PassHash, @PromedioCalificacionComprador, @CalificacionVendedor);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await connection.QuerySingleAsync<int>(sql, cliente);
    }

    public async Task<bool> UpdateAsync(Cliente cliente)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Clientes SET
                        nombre = @Nombre,
                        email = @Email,
                        pass_hash = @PassHash,
                        promedio_calificacion_comprador = @PromedioCalificacionComprador,
                        calificacion_vendedor = @CalificacionVendedor
                    WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, cliente);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "DELETE FROM Clientes WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
