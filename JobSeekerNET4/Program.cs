using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace JobSeekerNET4
{
    class Program
    {
        const string uri = "http://www.vagas.com.br";

        public static List<ResumoVaga> ListaResumos { get; set; }

        static void Main(string[] args)
        {
            ListaResumos = new List<ResumoVaga>();

            while (true)
            {
                Executar();

                Thread.Sleep(60000); // 60 segundos
            }
        }

        private static void Executar()
        {               
            var web = new HtmlWeb();
            var document = web.Load(uri + "/vagas-em-rio-de-janeiro?a%5B%5D=24&h%5B%5D=40&h%5B%5D=50&h%5B%5D=60");
            var vagas = document.DocumentNode.SelectNodes("//section[@name='vagasAbertasHaUmaSemana']//article//header");

            foreach (var vaga in vagas)
                AdicionarVaga(vaga);
        }

        private static void AdicionarVaga(HtmlNode vaga)
        {
            var dadosVaga = vaga.ChildNodes["h2"].ChildNodes["a"];
            var idVaga = dadosVaga.Attributes["id"]?.Value;
            var tituloVaga = dadosVaga.Attributes["title"]?.Value;
            var urlVaga = uri + dadosVaga.Attributes["href"]?.Value;

            var dadosEmpresa = vaga.ChildNodes["span"].ChildNodes.Where(x => x.Name == "span");
            var nomeEmpresa = dadosEmpresa.FirstOrDefault()?.ChildNodes["span"].InnerHtml;
            var nivelCargoEmpresa = dadosEmpresa.LastOrDefault()?.InnerHtml;

            var novoResumo = new ResumoVaga(idVaga, tituloVaga, urlVaga, nivelCargoEmpresa, nomeEmpresa);

            if (novoResumo.IsValid() && !ListaResumos.Any(x => x.ObterID() == idVaga))
                ListaResumos.Add(novoResumo);
        }
    }
}
