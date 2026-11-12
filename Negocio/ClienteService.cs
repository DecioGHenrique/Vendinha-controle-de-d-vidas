using Vendinha.Dados;
using Vendinha.Models;

namespace Vendinha.Negocio
{
    public class ClienteService
    {
        private const int TamanhoPagina = 10;

        private readonly ClienteRepository clienteRepositorio = new ClienteRepository();

        public List<Cliente> Listar(string busca, int pagina, out int totalPaginas)
        {
            var clientes = clienteRepositorio.ListarTodos();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var texto = busca.Trim();
                clientes = clientes
                    .Where(c => c.NomeCompleto.Contains(texto, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            totalPaginas = (int)Math.Ceiling(clientes.Count / (double)TamanhoPagina);

            if (pagina > totalPaginas)
            {
                pagina = totalPaginas;
            }

            if (pagina < 1)
            {
                pagina = 1;
            }

            return clientes
                .OrderByDescending(c => c.TotalDevido)
                .ThenBy(c => c.NomeCompleto)
                .Skip((pagina - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();
        }

        public Cliente? BuscarPorId(int id)
        {
            return clienteRepositorio.BuscarPorId(id);
        }

        public bool Cadastrar(string nomeCompleto, string cpf, DateTime dataNascimento, string email, out string mensagem)
        {
            if (!Validar(nomeCompleto, cpf, dataNascimento, email, 0, out mensagem))
            {
                return false;
            }

            var cliente = new Cliente
            {
                NomeCompleto = nomeCompleto.Trim(),
                Cpf = ValidadorCpf.SomenteNumeros(cpf),
                DataNascimento = dataNascimento.Date,
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim()
            };

            clienteRepositorio.Inserir(cliente);

            mensagem = "Cliente cadastrado com sucesso.";
            return true;
        }

        public bool Atualizar(int id, string nomeCompleto, string cpf, DateTime dataNascimento, string email, out string mensagem)
        {
            var cliente = clienteRepositorio.BuscarPorId(id);

            if (cliente == null)
            {
                mensagem = "Cliente não encontrado.";
                return false;
            }

            if (!Validar(nomeCompleto, cpf, dataNascimento, email, id, out mensagem))
            {
                return false;
            }

            cliente.NomeCompleto = nomeCompleto.Trim();
            cliente.Cpf = ValidadorCpf.SomenteNumeros(cpf);
            cliente.DataNascimento = dataNascimento.Date;
            cliente.Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

            clienteRepositorio.Atualizar(cliente);

            mensagem = "Cliente atualizado com sucesso.";
            return true;
        }

        public bool Excluir(int id, out string mensagem)
        {
            var cliente = clienteRepositorio.BuscarPorId(id);

            if (cliente == null)
            {
                mensagem = "Cliente não encontrado.";
                return false;
            }

            clienteRepositorio.Remover(id);

            mensagem = "Cliente excluído com sucesso.";
            return true;
        }

        private bool Validar(string nomeCompleto, string cpf, DateTime dataNascimento, string email, int idAtual, out string mensagem)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
            {
                mensagem = "O nome completo é obrigatório.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(cpf))
            {
                mensagem = "O CPF é obrigatório.";
                return false;
            }

            if (!ValidadorCpf.Validar(cpf))
            {
                mensagem = "O CPF informado é inválido.";
                return false;
            }

            if (dataNascimento.Date > DateTime.Today)
            {
                mensagem = "A data de nascimento não pode ser futura.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(email) && !ValidadorEmail.Validar(email))
            {
                mensagem = "O e-mail informado é inválido.";
                return false;
            }

            if (clienteRepositorio.CpfExiste(ValidadorCpf.SomenteNumeros(cpf), idAtual))
            {
                mensagem = "Já existe um cliente cadastrado com este CPF.";
                return false;
            }

            mensagem = "";
            return true;
        }
    }
}
