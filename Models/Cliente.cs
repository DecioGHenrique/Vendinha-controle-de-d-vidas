namespace Vendinha.Models
{
    public class Cliente
    {
        private int idade;

        public int Id { get; set; }
        public string NomeCompleto { get; set; } = "";
        public string Cpf { get; set; } = "";
        public DateTime DataNascimento { get; set; }
        public string? Email { get; set; }
        public List<Divida> Dividas { get; set; } = new List<Divida>();

        public int Idade
        {
            get
            {
                idade = DateTime.Today.Year - DataNascimento.Year;
                var x = DataNascimento.AddYears(idade);
                if (DateTime.Today < x)
                    idade--;
                return idade;
            }
        }

        public decimal TotalDevido
        {
            get
            {
                return Dividas.Where(d => !d.Paga).Sum(d => d.Valor);
            }
        }

        public bool TemDividaEmAberto()
        {
            return Dividas.Any(d => !d.Paga);
        }
    }
}
