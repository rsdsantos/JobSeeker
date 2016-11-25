using HtmlAgilityPack;
using System;
using System.Data;
using System.Linq;
using System.Threading;

namespace JobSeekerNET4
{
    class Program
    {
        const string uri = "http://www.vagas.com.br";
        const string pathRepository = @"C:\Vagas\Vagas.xml";

        public static DataSet Repository { get; set; }
        public static int VagasNovas { get; set; }
        public static int VagasEncontradas { get; set; }

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    ConfigurarRepository();

                    Executar();

                    RedefinirContadores();

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

        private static void ConfigurarRepository()
        {
            Repository = new DataSet();
            Repository.ReadXml(pathRepository);            
        }

        private static void Executar()
        {
            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - Verificando novas vagas...");

            var web = new HtmlWeb();
            var document = web.Load(uri + "/vagas-em-rio-de-janeiro?a%5B%5D=24&h%5B%5D=40&h%5B%5D=50&h%5B%5D=60");
            var vagas = document.DocumentNode.SelectNodes("//section[@class='grupoDeVagas']//article//header");

            foreach (var vaga in vagas)
                AdicionarVaga(vaga);

            Salvar();

            Console.WriteLine("  -> Encontradas: " + VagasEncontradas);
            Console.WriteLine("  -> Novas: " + VagasNovas);
            Console.WriteLine("  -> Total: " + Repository.Tables["ResumoVaga"].AsEnumerable().Count());
            Console.WriteLine();
        }

        private static void RedefinirContadores()
        {
            VagasEncontradas = 0;
            VagasNovas = 0;
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

            var vagaExiste = VagaExiste(novoResumo);

            if (novoResumo.IsValid() && !vagaExiste)
            {
                AdicionarXML(novoResumo);
                VagasNovas++;
            }

            VagasEncontradas++;               
        }

        private static void AdicionarXML(ResumoVaga novoResumo)
        {
            var tabela = Repository.Tables["ResumoVaga"];

            DataRow dr = tabela.NewRow();
            dr["ID"] = novoResumo.ObterID();
            dr["Titulo"] = novoResumo.ObterTitulo();
            dr["URL"] = novoResumo.ObterURL();
            dr["NivelCargo"] = novoResumo.ObterNivelCargo();
            dr["Empresa"] = novoResumo.ObterEmpresa();
            dr["DataCadastro"] = novoResumo.ObterDataCadastro();

            tabela.Rows.Add(dr);            
        }

        private static bool VagaExiste(ResumoVaga novoResumo)
        {
            return Repository.Tables["ResumoVaga"]
                .AsEnumerable()
                .Any(x => x.Field<string>("ID") == novoResumo.ObterID());
        }

        private static void Salvar()
        {
            Repository.WriteXml(pathRepository);
        }
    }
}
