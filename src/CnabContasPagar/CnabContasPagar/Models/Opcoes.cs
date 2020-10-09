

using System;

namespace CnabContasPagar.Models
{
    public class Opcoes
    {
        public string CnpjPagador { get; set; }
        public string NumeroAgencia { get; set; }
        public string NumeroContaCorrente { get; set; }
        public char DAC { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeBanco { get; set; }
    }

}
