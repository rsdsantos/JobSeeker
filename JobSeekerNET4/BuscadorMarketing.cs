namespace JobSeekerNET4
{
    public class BuscadorMarketing : Buscador
    {
        public BuscadorMarketing(string filtro = @"/vagas-em-rio-de-janeiro?a%5B%5D=12&a%5B%5D=27&a%5B%5D=67&a%5B%5D=32&a%5B%5D=59", 
                          string pathRepository = @"C:\Vagas\VagasMarketing.xml") 
            : base(filtro, pathRepository)
        {
            nomeSistema = "MARKETING";
        }
    }
}
