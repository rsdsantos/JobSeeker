using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSeekerNET4
{
    public static class StringExtensions
    {
        public static string LimparConteudo(this string self)
        {
            if (string.IsNullOrWhiteSpace(self))
                return string.Empty;
            
            // Remove tabulação e quebra de linha.
            var txt = self.Replace("/n", string.Empty);
            
            // Remove espaços em branco nas extremidades.
            txt = txt.Trim();

            return txt;
        }
    }
}
