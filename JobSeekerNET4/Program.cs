using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace JobSeekerNET4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " :: Monitoramento de vagas em andamento...");

            while (true)
            {
                try
                {
                    var buscadores = new List<Buscador>
                    {
                        new BuscadorTI(),
                        new BuscadorMarketing()
                    };

                    foreach (var buscador in buscadores)
                        buscador.Executar();                    

                    Thread.Sleep(60000); // 60 segundos
                }
                catch
                {
                    Console.WriteLine("Erro ao consultar vagas. Tentando novamente em alguns minutos...");
                    Console.WriteLine();

                    Thread.Sleep(300000); // 5 minutos
                }                
            }
        }
    }
}
