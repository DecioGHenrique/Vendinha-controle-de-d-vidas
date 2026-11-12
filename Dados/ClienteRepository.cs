using Microsoft.Data.Sqlite;
using Vendinha.Models;

namespace Vendinha.Dados
{
    public class ClienteRepository
    {
        private readonly DividaRepository dividaRepositorio = new DividaRepository();

        public List<Cliente> ListarTodos()
        {
            var clientes = LerClientes();
            var dividas = dividaRepositorio.ListarTodas();

            foreach (var divida in dividas)
            {
                var cliente = clientes.First(c => c.Id == divida.ClienteId);
                cliente.Dividas.Add(divida);
            }

            return clientes;
        }

        public Cliente? BuscarPorId(int id)
        {
            string sql = "SELECT Id, NomeCompleto, Cpf, DataNascimento, Email FROM Cliente WHERE Id = @Id";
            Cliente? cliente = null;

            using (var connection = new SqliteConnection(Conexao.ConnectionString))
            {
                connection.Open();

                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    cliente = LerCliente(reader);
                }
            }

            if (cliente != null)
            {
                cliente.Dividas = dividaRepositorio.ListarPorCliente(id);
            }

            return cliente;
        }

        public bool CpfExiste(string cpf, int idIgnorado = 0)
        {
            string sql = "SELECT Id FROM Cliente WHERE Cpf = @Cpf AND Id <> @Id";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@Cpf", cpf);
            command.Parameters.AddWithValue("@Id", idIgnorado);

            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        public int Inserir(Cliente cliente)
        {
            string sql = "INSERT INTO Cliente (NomeCompleto, Cpf, DataNascimento, Email) " +
                         "VALUES (@NomeCompleto, @Cpf, @DataNascimento, @Email) RETURNING Id";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@NomeCompleto", cliente.NomeCompleto);
            command.Parameters.AddWithValue("@Cpf", cliente.Cpf);
            command.Parameters.AddWithValue("@DataNascimento", cliente.DataNascimento);
            command.Parameters.AddWithValue("@Email", (object?)cliente.Email ?? DBNull.Value);

            using var reader = command.ExecuteReader();
            reader.Read();
            return reader.GetInt32(0);
        }

        public void Atualizar(Cliente cliente)
        {
            string sql = "UPDATE Cliente SET NomeCompleto = @NomeCompleto, Cpf = @Cpf, " +
                         "DataNascimento = @DataNascimento, Email = @Email WHERE Id = @Id";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@NomeCompleto", cliente.NomeCompleto);
            command.Parameters.AddWithValue("@Cpf", cliente.Cpf);
            command.Parameters.AddWithValue("@DataNascimento", cliente.DataNascimento);
            command.Parameters.AddWithValue("@Email", (object?)cliente.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", cliente.Id);

            command.ExecuteNonQuery();
        }

        public void Remover(int id)
        {
            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                using var apagaDividas = new SqliteCommand(
                    "DELETE FROM Divida WHERE ClienteId = @ClienteId", connection, transaction);
                apagaDividas.Parameters.AddWithValue("@ClienteId", id);
                apagaDividas.ExecuteNonQuery();

                using var apagaCliente = new SqliteCommand(
                    "DELETE FROM Cliente WHERE Id = @Id", connection, transaction);
                apagaCliente.Parameters.AddWithValue("@Id", id);
                apagaCliente.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private List<Cliente> LerClientes()
        {
            var clientes = new List<Cliente>();
            string sql = "SELECT Id, NomeCompleto, Cpf, DataNascimento, Email FROM Cliente";

            using var connection = new SqliteConnection(Conexao.ConnectionString);
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                clientes.Add(LerCliente(reader));
            }

            return clientes;
        }

        private Cliente LerCliente(SqliteDataReader reader)
        {
            return new Cliente
            {
                Id = reader.GetInt32(0),
                NomeCompleto = reader.GetString(1),
                Cpf = reader.GetString(2),
                DataNascimento = reader.GetDateTime(3),
                Email = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
        }
    }
}
