using HtmlAgilityPack;
using System;
using System.Data;
using System.Linq;

namespace JobSeekerNET4
{
    public abstract class Buscador
    {
        const string uri = "http://www.vagas.com.br";
        protected string nomeSistema;

        public string filtro { get; private set; }
        public string pathRepository { get; private set; }
        
        public DataSet Repository { get; private set; }
        public int VagasNovas { get; private set; }
        public int VagasEncontradas { get; private set; }

        public Buscador(string filtro, string pathRepository)
        {
            this.filtro = filtro;
            this.pathRepository = pathRepository;

            ConfigurarRepository();
        }

        private void ConfigurarRepository()
        {
            Repository = new DataSet();
            Repository.ReadXml(pathRepository);

            if (Repository.Tables["ResumoVaga"] == null)
                CriarDataSet();
        }

        private void CriarDataSet()
        {
            var novaTabela = new DataTable("ResumoVaga");

            novaTabela.Columns.Add(new DataColumn("ID", typeof(string)));
            novaTabela.Columns.Add(new DataColumn("Titulo", typeof(string)));
            novaTabela.Columns.Add(new DataColumn("URL", typeof(string)));
            novaTabela.Columns.Add(new DataColumn("NivelCargo", typeof(string)));
            novaTabela.Columns.Add(new DataColumn("Empresa", typeof(string)));
            novaTabela.Columns.Add(new DataColumn("DataCadastro", typeof(DateTime)));

            Repository.Tables.Add(novaTabela);
        }

        public void Executar()
        {           
            var web = new HtmlWeb();
            var document = web.Load(uri + filtro);
            var vagas = document.DocumentNode.SelectNodes("//section[@class='grupoDeVagas']//article//header");

            foreach (var vaga in vagas)
                AdicionarVaga(vaga);

            Salvar();

            if (VagasNovas > 0)
            {
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - Novas vagas de " + nomeSistema + " encontradas.");
                Console.WriteLine("  -> Encontradas: " + VagasEncontradas);
                Console.WriteLine("  -> Novas: " + VagasNovas);
                Console.WriteLine("  -> Total: " + Repository.Tables["ResumoVaga"].AsEnumerable().Count());
                Console.WriteLine();
            }                  

            RedefinirContadores();
        }

        private void RedefinirContadores()
        {
            VagasEncontradas = 0;
            VagasNovas = 0;
        }

        private void AdicionarVaga(HtmlNode vaga)
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

        private void AdicionarXML(ResumoVaga novoResumo)
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

        private bool VagaExiste(ResumoVaga novoResumo)
        {
            return Repository.Tables["ResumoVaga"]
                .AsEnumerable()
                .Any(x => x.Field<string>("ID") == novoResumo.ObterID());
        }

        private void Salvar()
        {
            Repository.WriteXml(pathRepository);
        }
    }
}
