using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Models
{
    class Liquidacao
    {
        public string FormaPagamento { get; set; }
        public string BancoFavorecido { get; set; }
        public string AgenciaFavorecido { get; set; }
        public string ContaFavorecido { get; set; }
        public string DacFavorecido { get; set; }
        public string NomeFavorecido { get; set; }
        public string NossoNumero { get; set; }
    }
}
