using Vendinha.Dados;
using Vendinha.Models;

namespace Vendinha.Negocio
{
    public class DividaService
    {
        private const int TamanhoPagina = 10;

        private readonly ClienteRepository clienteRepositorio = new ClienteRepository();
        private readonly DividaRepository dividaRepositorio = new DividaRepository();

        public List<Divida> ListarPorCliente(int clienteId, int pagina, out int totalPaginas)
        {
            var dividas = dividaRepositorio.ListarPorCliente(clienteId);

            totalPaginas = (int)Math.Ceiling(dividas.Count / (double)TamanhoPagina);

            if (pagina > totalPaginas)
            {
                pagina = totalPaginas;
            }

            if (pagina < 1)
            {
                pagina = 1;
            }

            return dividas
                .OrderByDescending(d => d.DataCriacao)
                .Skip((pagina - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();
        }

        public bool Cadastrar(int clienteId, decimal valor, out string mensagem)
        {
            var cliente = clienteRepositorio.BuscarPorId(clienteId);

            if (cliente == null)
            {
                mensagem = "Cliente não encontrado.";
                return false;
            }

            if (valor <= 0)
            {
                mensagem = "O valor da dívida deve ser maior que zero.";
                return false;
            }

            if (cliente.TemDividaEmAberto())
            {
                mensagem = "O cliente já possui uma dívida em aberto.";
                return false;
            }

            var divida = new Divida
            {
                ClienteId = clienteId,
                Valor = Math.Round(valor, 2),
                Paga = false,
                DataCriacao = DateTime.Now
            };

            dividaRepositorio.Inserir(divida);

            mensagem = "Dívida cadastrada com sucesso.";
            return true;
        }

        public bool MarcarComoPaga(int clienteId, int dividaId, out string mensagem)
        {
            var cliente = clienteRepositorio.BuscarPorId(clienteId);

            if (cliente == null)
            {
                mensagem = "Cliente não encontrado.";
                return false;
            }

            var divida = cliente.Dividas.FirstOrDefault(d => d.Id == dividaId);

            if (divida == null)
            {
                mensagem = "Dívida não encontrada para este cliente.";
                return false;
            }

            if (divida.Paga)
            {
                mensagem = "Esta dívida já está paga.";
                return false;
            }

            divida.Paga = true;
            divida.DataPagamento = DateTime.Now;
            dividaRepositorio.RegistrarPagamento(divida);

            mensagem = "Dívida marcada como paga.";
            return true;
        }
    }
}
