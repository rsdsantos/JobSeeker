using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSeekerNET4
{
    public class ResumoVaga
    {        
        private string ID { get; set; }
        private string Titulo { get; set; }
        private string URL { get; set; }
        private string NivelCargo { get; set; }
        private string Empresa { get; set; }
        private DateTime DataCadastro { get; set; }

        public ResumoVaga(string iD, string titulo, string uRL, string nivelCargo, string empresa)
        {
            ID = iD.LimparConteudo();
            Titulo = titulo.LimparConteudo();
            URL = uRL.LimparConteudo();
            NivelCargo = nivelCargo.LimparConteudo();
            Empresa = empresa.LimparConteudo();
            DataCadastro = DateTime.Now;
        }

        internal bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ID))
                return false;

            if (string.IsNullOrWhiteSpace(URL))
                return false;

            return true;
        }

        public string ObterID()
        {
            return ID;
        }

        public string ObterTitulo()
        {
            return Titulo.LimparConteudo();
        }

        public string ObterURL()
        {
            return URL;
        }

        public string ObterNivelCargo()
        {
            return NivelCargo.LimparConteudo();
        }

        public string ObterEmpresa()
        {
            return Empresa.LimparConteudo();
        }

        public DateTime ObterDataCadastro()
        {
            return DataCadastro;
        }
    }
}
