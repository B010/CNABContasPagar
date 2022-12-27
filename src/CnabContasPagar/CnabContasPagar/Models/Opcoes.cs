using System;

namespace CnabContasPagar.Models
{
    public class Opcoes
    {
        public string CnpjPagador { get; set; }
        public string EnderecoPagador { get; set; }
        public string NumeroAgencia { get; set; }
        public string DigitoAgencia { get; set; }
        public string NumeroContaCorrente { get; set; }
        public string DAC { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeBanco { get; set; }
        public string NumeroConvenio { get; set; }

        public string Numero { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }

        public string NumeroSequencial { get; set; }
    }
}