using Microsoft.Data.Sqlite;
using Vendinha.Models;

namespace Vendinha.Dados
{
    public class DividaRepository
    {
        public List<Divida> ListarTodas()
        {
            string sql = "SELECT Id, ClienteId, Valor, Paga, DataCriacao, DataPagamento FROM Divida";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            return LerDividas(command);
        }

        public List<Divida> ListarPorCliente(int clienteId)
        {
            string sql = "SELECT Id, ClienteId, Valor, Paga, DataCriacao, DataPagamento " +
                         "FROM Divida WHERE ClienteId = @ClienteId";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@ClienteId", clienteId);

            return LerDividas(command);
        }

        public int Inserir(Divida divida)
        {
            string sql = "INSERT INTO Divida (ClienteId, Valor, Paga, DataCriacao) " +
                         "VALUES (@ClienteId, @Valor, 0, @DataCriacao) RETURNING Id";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@ClienteId", divida.ClienteId);
            command.Parameters.AddWithValue("@Valor", divida.Valor);
            command.Parameters.AddWithValue("@DataCriacao", divida.DataCriacao);

            using var reader = command.ExecuteReader();
            reader.Read();
            return reader.GetInt32(0);
        }

        public int RegistrarPagamento(Divida divida)
        {
            string sql = "UPDATE Divida SET Paga = 1, DataPagamento = @DataPagamento WHERE Id = @Id";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@DataPagamento", divida.DataPagamento ?? DateTime.Now);
            command.Parameters.AddWithValue("@Id", divida.Id);

            int linhasAfetadas = command.ExecuteNonQuery();
            return linhasAfetadas;
        }

        private List<Divida> LerDividas(SqliteCommand command)
        {
            var dividas = new List<Divida>();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var divida = new Divida
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    Valor = reader.GetDecimal(2),
                    Paga = reader.GetBoolean(3),
                    DataCriacao = reader.GetDateTime(4),
                    DataPagamento = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
                };

                dividas.Add(divida);
            }

            return dividas;
        }
    }
}
