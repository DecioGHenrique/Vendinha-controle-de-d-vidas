using System.Globalization;
using Microsoft.Data.Sqlite;
using Vendinha.Models;
using Vendinha.Negocio;

namespace Vendinha.Apresentacao
{
    public class Menu
    {
        private readonly ClienteService servicoCliente = new ClienteService();
        private readonly DividaService servicoDivida = new DividaService();

        public void Executar()
        {
            var opcao = OpcaoMenu.Invalida;

            do
            {
                ExibirMenu();
                opcao = LerOpcao();

                try
                {
                    switch (opcao)
                    {
                        case OpcaoMenu.CadastrarCliente:
                            CadastrarCliente();
                            break;
                        case OpcaoMenu.ListarClientes:
                            ListarClientes();
                            break;
                        case OpcaoMenu.DetalharCliente:
                            DetalharCliente();
                            break;
                        case OpcaoMenu.AtualizarCliente:
                            AtualizarCliente();
                            break;
                        case OpcaoMenu.ExcluirCliente:
                            ExcluirCliente();
                            break;
                        case OpcaoMenu.CadastrarDivida:
                            CadastrarDivida();
                            break;
                        case OpcaoMenu.ListarDividas:
                            ListarDividas();
                            break;
                        case OpcaoMenu.Sair:
                            Console.WriteLine("Encerrando o sistema.");
                            break;
                        case OpcaoMenu.Invalida:
                            break;
                    }
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"Erro ao acessar o banco de dados: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
                }
            } while (opcao != OpcaoMenu.Sair);
        }

        private void ExibirMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== VENDINHA =====");
            Console.WriteLine("1 - Cadastrar cliente");
            Console.WriteLine("2 - Listar clientes");
            Console.WriteLine("3 - Detalhes do cliente");
            Console.WriteLine("4 - Atualizar cliente");
            Console.WriteLine("5 - Excluir cliente");
            Console.WriteLine("6 - Cadastrar dívida");
            Console.WriteLine("7 - Dívidas do cliente (listar e pagar)");
            Console.WriteLine("0 - Sair");
        }

        private OpcaoMenu LerOpcao()
        {
            Console.Write("Escolha uma opção: ");
            var texto = (Console.ReadLine() ?? "").Trim();

            if (texto == "")
            {
                Console.WriteLine("Você não digitou nada. Digite o número de uma opção.");
                return OpcaoMenu.Invalida;
            }

            if (!int.TryParse(texto, out int numero))
            {
                Console.WriteLine("Opção inválida. Digite apenas números.");
                return OpcaoMenu.Invalida;
            }

            if (numero < 0 || !Enum.IsDefined(typeof(OpcaoMenu), numero))
            {
                Console.WriteLine("Este número não existe no menu. Escolha uma opção de 0 a 7.");
                return OpcaoMenu.Invalida;
            }

            return (OpcaoMenu)numero;
        }

        private bool LerInteiro(string pergunta, out int numero)
        {
            numero = 0;

            while (true)
            {
                Console.Write(pergunta);
                var texto = (Console.ReadLine() ?? "").Trim();

                if (texto == "")
                {
                    Console.WriteLine("Operação cancelada.");
                    return false;
                }

                if (int.TryParse(texto, out numero))
                {
                    return true;
                }

                Console.WriteLine("Valor inválido. Digite apenas números.");
            }
        }

        private bool LerDecimal(string pergunta, out decimal valor)
        {
            valor = 0;

            while (true)
            {
                Console.Write(pergunta);
                var texto = (Console.ReadLine() ?? "").Trim();

                if (texto == "")
                {
                    Console.WriteLine("Operação cancelada.");
                    return false;
                }

                if (decimal.TryParse(texto.Replace(',', '.'), NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out valor))
                {
                    return true;
                }

                Console.WriteLine("Valor inválido. Digite um número, por exemplo 49,90.");
            }
        }

        private bool LerData(string pergunta, out DateTime data)
        {
            data = DateTime.MinValue;

            while (true)
            {
                Console.Write(pergunta);
                var texto = (Console.ReadLine() ?? "").Trim();

                if (texto == "")
                {
                    Console.WriteLine("Operação cancelada.");
                    return false;
                }

                if (DateTime.TryParseExact(texto, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out data))
                {
                    return true;
                }

                Console.WriteLine("Data inválida. Use o formato dd/MM/yyyy, por exemplo 10/05/2000.");
            }
        }

        private bool LerNome(out string nome)
        {
            Console.Write("Nome completo (Enter para cancelar): ");
            nome = (Console.ReadLine() ?? "").Trim();

            if (nome == "")
            {
                Console.WriteLine("Operação cancelada.");
                return false;
            }

            return true;
        }

        private bool LerCpf(out string cpf)
        {
            cpf = "";

            while (true)
            {
                Console.Write("CPF (Enter para cancelar): ");
                var texto = (Console.ReadLine() ?? "").Trim();

                if (texto == "")
                {
                    Console.WriteLine("Operação cancelada.");
                    return false;
                }

                if (ValidadorCpf.Validar(texto))
                {
                    cpf = texto;
                    return true;
                }

                Console.WriteLine("CPF inválido. Confira os números e tente novamente.");
            }
        }

        private string LerEmail()
        {
            while (true)
            {
                Console.Write("E-mail (opcional, Enter para deixar em branco): ");
                var texto = (Console.ReadLine() ?? "").Trim();

                if (texto == "" || ValidadorEmail.Validar(texto))
                {
                    return texto;
                }

                Console.WriteLine("E-mail inválido. Digite novamente ou deixe em branco.");
            }
        }

        private bool LerDadosCliente(out string nome, out string cpf, out DateTime dataNascimento, out string email)
        {
            nome = "";
            cpf = "";
            dataNascimento = DateTime.MinValue;
            email = "";

            if (!LerNome(out nome))
            {
                return false;
            }

            if (!LerCpf(out cpf))
            {
                return false;
            }

            if (!LerData("Data de nascimento dd/MM/yyyy (Enter para cancelar): ", out dataNascimento))
            {
                return false;
            }

            email = LerEmail();

            return true;
        }

        private string FormatarCpf(string cpf)
        {
            if (long.TryParse(cpf, out long numero))
            {
                return numero.ToString(@"000\.000\.000\-00");
            }

            return cpf;
        }

        private bool Navegar(string comando, ref int pagina, int totalPaginas)
        {
            if (comando == "P")
            {
                if (pagina < totalPaginas)
                {
                    pagina++;
                }
                else
                {
                    Console.WriteLine("Você já está na última página.");
                }

                return true;
            }

            if (comando == "A")
            {
                if (pagina > 1)
                {
                    pagina--;
                }
                else
                {
                    Console.WriteLine("Você já está na primeira página.");
                }

                return true;
            }

            return false;
        }

        private Cliente? EscolherCliente()
        {
            Console.Write("Digite parte do nome do cliente (Enter para cancelar): ");
            var busca = (Console.ReadLine() ?? "").Trim();

            if (busca == "")
            {
                Console.WriteLine("Operação cancelada.");
                return null;
            }

            var encontrados = servicoCliente.Listar(busca, 1, out int totalPaginas);

            if (encontrados.Count == 0)
            {
                Console.WriteLine("Nenhum cliente encontrado com esse nome.");
                return null;
            }

            if (encontrados.Count == 1)
            {
                Console.WriteLine("Cliente selecionado: [ID: {0}] {1}", encontrados[0].Id, encontrados[0].NomeCompleto);
                return encontrados[0];
            }

            foreach (var c in encontrados)
            {
                Console.WriteLine("[ID: {0}] {1}", c.Id, c.NomeCompleto);
            }

            if (totalPaginas > 1)
            {
                Console.WriteLine("Existem mais clientes com esse nome. Digite um nome mais completo para refinar.");
            }

            while (true)
            {
                if (!LerInteiro("Digite o ID do cliente desejado (Enter para cancelar): ", out int id))
                {
                    return null;
                }

                var escolhido = encontrados.FirstOrDefault(c => c.Id == id);

                if (escolhido != null)
                {
                    return escolhido;
                }

                Console.WriteLine("Este ID não está na lista acima. Tente novamente.");
            }
        }

        private void CadastrarCliente()
        {
            if (!LerDadosCliente(out string nome, out string cpf, out DateTime dataNascimento, out string email))
            {
                return;
            }

            servicoCliente.Cadastrar(nome, cpf, dataNascimento, email, out string mensagem);
            Console.WriteLine(mensagem);
        }

        private void ListarClientes()
        {
            Console.Write("Buscar por nome (Enter para listar todos): ");
            var busca = (Console.ReadLine() ?? "").Trim();

            var pagina = 1;
            var comando = "";

            do
            {
                var clientes = servicoCliente.Listar(busca, pagina, out int totalPaginas);

                if (clientes.Count == 0)
                {
                    Console.WriteLine("Nenhum cliente encontrado.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("--- Página {0} de {1} ---", pagina, totalPaginas);

                foreach (var c in clientes)
                {
                    Console.WriteLine("[ID: {0}] {1} - CPF: {2} - {3} anos - Total devido: {4:C}",
                        c.Id, c.NomeCompleto, FormatarCpf(c.Cpf), c.Idade, c.TotalDevido);
                }

                Console.Write("[P] próxima  [A] anterior  [V] voltar: ");
                comando = (Console.ReadLine() ?? "").Trim().ToUpper();

                if (!Navegar(comando, ref pagina, totalPaginas) && comando != "V")
                {
                    Console.WriteLine("Comando inválido. Digite P, A ou V.");
                }
            } while (comando != "V");
        }

        private void DetalharCliente()
        {
            var cliente = EscolherCliente();

            if (cliente == null)
            {
                return;
            }

            Console.WriteLine("[ID: {0}] {1}", cliente.Id, cliente.NomeCompleto);
            Console.WriteLine("CPF: {0}", FormatarCpf(cliente.Cpf));
            Console.WriteLine("Data de nascimento: {0:dd/MM/yyyy} ({1} anos)", cliente.DataNascimento, cliente.Idade);
            Console.WriteLine("E-mail: {0}", cliente.Email ?? "(não informado)");
            Console.WriteLine("Total devido: {0:C}", cliente.TotalDevido);
        }

        private void AtualizarCliente()
        {
            var cliente = EscolherCliente();

            if (cliente == null)
            {
                return;
            }

            Console.WriteLine("Digite os novos dados do cliente.");

            if (!LerDadosCliente(out string nome, out string cpf, out DateTime dataNascimento, out string email))
            {
                return;
            }

            servicoCliente.Atualizar(cliente.Id, nome, cpf, dataNascimento, email, out string mensagem);
            Console.WriteLine(mensagem);
        }

        private void ExcluirCliente()
        {
            var cliente = EscolherCliente();

            if (cliente == null)
            {
                return;
            }

            Console.Write("O cliente e suas dívidas serão apagados. Confirma? (S/N): ");
            var confirmacao = (Console.ReadLine() ?? "").Trim().ToUpper();

            if (confirmacao != "S")
            {
                Console.WriteLine("Operação cancelada.");
                return;
            }

            servicoCliente.Excluir(cliente.Id, out string mensagem);
            Console.WriteLine(mensagem);
        }

        private void CadastrarDivida()
        {
            var cliente = EscolherCliente();

            if (cliente == null)
            {
                return;
            }

            if (!LerDecimal("Valor da dívida (Enter para cancelar): ", out decimal valor))
            {
                return;
            }

            servicoDivida.Cadastrar(cliente.Id, valor, out string mensagem);
            Console.WriteLine(mensagem);
        }

        private void ListarDividas()
        {
            var cliente = EscolherCliente();

            if (cliente == null)
            {
                return;
            }

            var pagina = 1;
            var comando = "";

            do
            {
                var dividas = servicoDivida.ListarPorCliente(cliente.Id, pagina, out int totalPaginas);

                Console.WriteLine();
                Console.WriteLine("Dívidas de [ID: {0}] {1}", cliente.Id, cliente.NomeCompleto);

                if (dividas.Count == 0)
                {
                    Console.WriteLine("Nenhuma dívida cadastrada.");
                    return;
                }

                Console.WriteLine("--- Página {0} de {1} ---", pagina, totalPaginas);

                foreach (var d in dividas)
                {
                    var situacao = d.Paga ? "Paga" : "Em aberto";
                    var pagamento = d.DataPagamento.HasValue
                        ? d.DataPagamento.Value.ToString("dd/MM/yyyy HH:mm")
                        : "-";

                    Console.WriteLine("[ID: {0}] {1:C} - {2} - Criada em {3:dd/MM/yyyy HH:mm} - Pagamento: {4}",
                        d.Id, d.Valor, situacao, d.DataCriacao, pagamento);
                }

                Console.Write("[P] próxima  [A] anterior  [B] marcar como paga  [V] voltar: ");
                comando = (Console.ReadLine() ?? "").Trim().ToUpper();

                if (comando == "B")
                {
                    PagarDivida(cliente.Id);
                }
                else if (!Navegar(comando, ref pagina, totalPaginas) && comando != "V")
                {
                    Console.WriteLine("Comando inválido. Digite P, A, B ou V.");
                }
            } while (comando != "V");
        }

        private void PagarDivida(int clienteId)
        {
            if (!LerInteiro("ID da dívida a marcar como paga (Enter para cancelar): ", out int dividaId))
            {
                return;
            }

            servicoDivida.MarcarComoPaga(clienteId, dividaId, out string mensagem);
            Console.WriteLine(mensagem);
        }
    }
}
