using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Models
{
    public class Liquidacao
    {
        public string FormaPagamento { get; set; }
        public string BancoFavorecido { get; set; }
        public string AgenciaFavorecido { get; set; }
        public string ContaFavorecido { get; set; }
        public string DacFavorecido { get; set; }
        public string NomeFavorecido { get; set; }
        public string NossoNumero { get; set; }
        public DateTime DataPagamento { get; set; }
        public decimal ValorPagamento { get; set; }
        public string CpfCnpjFavorecido { get; set; }
    }
}
