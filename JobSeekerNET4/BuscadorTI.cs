namespace JobSeekerNET4
{
    public class BuscadorTI : Buscador
    {
        public BuscadorTI(string filtro = @"/vagas-em-rio-de-janeiro?a%5B%5D=24&h%5B%5D=40&h%5B%5D=50&h%5B%5D=60", 
                          string pathRepository = @"C:\Vagas\VagasTI.xml") 
            : base(filtro, pathRepository)
        {
            nomeSistema = "TI";
        }
    }
}
