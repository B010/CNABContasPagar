using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Models
{
    public class Liquidacao
    {
        public string TipoPagamento { get; set; } // ??? Fornecedores - 20
        public string FormaPagamento { get; set; } // ??? Por enquanto vai ser Credito em Conta Itau - 01
        public string BancoFavorecido { get; set; }
        public string AgenciaFavorecido { get; set; }
        public string ContaFavorecido { get; set; }
        public string DacFavorecido { get; set; } // ???
        public string NomeFavorecido { get; set; }
        public string NossoNumero { get; set; } // Não tem para Arquivo Remessa
        public DateTime DataPagamento { get; set; } // ??? Data da Criação do Arquivo
        public decimal ValorPagamento { get; set; }
        public string CpfCnpjFavorecido { get; set; }
        public string Documento { get; set; }
    }
}
