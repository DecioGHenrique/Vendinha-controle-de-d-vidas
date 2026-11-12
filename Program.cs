using System.Text;
using Vendinha.Apresentacao;
using Vendinha.Dados;

Console.OutputEncoding = Encoding.UTF8;

BancoDeDados.Criar();

var menu = new Menu();
menu.Executar();
