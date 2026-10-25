using System.Data;
using Microsoft.EntityFrameworkCore;
using MiniPosAi.Data;

namespace MiniPosAi.Services;

public class QueryExecutor(AppDbContext db)
{
    public async Task<List<Dictionary<string, object?>>> ExecuteSelectAsync(string sql, CancellationToken cancellationToken = default)
    {
        if (!SqlValidator.IsSafeSelect(sql, out var error))
            throw new InvalidOperationException(error);

        await using var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.CommandTimeout = 30;

        var rows = new List<Dictionary<string, object?>>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                row[reader.GetName(i)] = value is DBNull ? null : value;
            }
            rows.Add(row);
        }

        return rows;
    }
}
