namespace Vendinha.Dados
{
    public static class Conexao
    {
        public static string ConnectionString =
            "Data Source=" + Path.Combine(AppContext.BaseDirectory, "vendinha.db") + ";Foreign Keys=True";
    }
}
