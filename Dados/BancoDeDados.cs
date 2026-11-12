using Microsoft.Data.Sqlite;

namespace Vendinha.Dados
{
    public static class BancoDeDados
    {
        public static void Criar()
        {
            var caminho = Path.Combine(AppContext.BaseDirectory, "script.sql");
            var sql = File.ReadAllText(caminho);

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
