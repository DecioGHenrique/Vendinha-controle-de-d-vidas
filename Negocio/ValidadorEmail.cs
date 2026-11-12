namespace Vendinha.Negocio
{
    public static class ValidadorEmail
    {
        public static bool Validar(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            email = email.Trim();

            if (email.Contains(' '))
            {
                return false;
            }

            var partes = email.Split('@');

            if (partes.Length != 2 || partes[0].Length == 0)
            {
                return false;
            }

            var dominio = partes[1];

            return dominio.Contains('.') && !dominio.StartsWith('.') && !dominio.EndsWith('.');
        }
    }
}
