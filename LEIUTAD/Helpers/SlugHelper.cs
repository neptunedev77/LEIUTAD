using System.Globalization;
using System.Linq;

namespace LEIUTAD.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string input)
        {
            // Remove acentos e caracteres especiais
            string normalized = input
                .Normalize(System.Text.NormalizationForm.FormD) // Normaliza o texto
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate("", (current, c) => current + c);

            // Substituir espaços por traços e converter para minúsculas
            return normalized
                .ToLower()
                .Replace(" ", "-") // Substituir espaços
                .Replace("ç", "c")
                .Replace("ã", "a")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ê", "e")
                .Replace("ô", "o")
                .Replace("ü", "u")
                .Replace("&", "e")
                .Replace("'", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("/", "-")
                .Replace("(", "")
                .Replace(")", "")
                .Trim('-'); // Remove traços extras no início ou fim
        }
    }
}
