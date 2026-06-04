using Dapper;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Infrastructure.Repositories;

public class TransaccionesRepository : ITransaccionesRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public TransaccionesRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Transaccion>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Transaccion>("SELECT * FROM Transacciones");
    }

    public async Task<Transaccion?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Transaccion>(
            "SELECT * FROM Transacciones WHERE id = @Id", new { Id = id });
    }

    public async Task<int> AddAsync(Transaccion transaccion)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Transacciones (oferta_id, cliente_comprador_id, estado) 
            VALUES (@oferta_id, @cliente_comprador_id, @estado); 
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await connection.QuerySingleAsync<int>(sql, transaccion);
    }

    public async Task<bool> UpdateAsync(Transaccion transaccion)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Transacciones SET
                    estado = @estado
                WHERE id = @id";

        var rowsAffected = await connection.ExecuteAsync(sql, transaccion);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "DELETE FROM Transacciones WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
