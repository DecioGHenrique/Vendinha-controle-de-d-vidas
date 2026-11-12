namespace Vendinha.Negocio
{
    public static class ValidadorCpf
    {
        public static string SomenteNumeros(string cpf)
        {
            return new string(cpf.Where(char.IsDigit).ToArray());
        }

        public static bool Validar(string cpf)
        {
            var numeros = SomenteNumeros(cpf);

            if (numeros.Length != 11)
            {
                return false;
            }

            if (numeros.Distinct().Count() == 1)
            {
                return false;
            }

            var soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += (numeros[i] - '0') * (10 - i);
            }

            var resto = soma % 11;
            var primeiroDigito = resto < 2 ? 0 : 11 - resto;

            if (primeiroDigito != numeros[9] - '0')
            {
                return false;
            }

            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += (numeros[i] - '0') * (11 - i);
            }

            resto = soma % 11;
            var segundoDigito = resto < 2 ? 0 : 11 - resto;

            return segundoDigito == numeros[10] - '0';
        }
    }
}
