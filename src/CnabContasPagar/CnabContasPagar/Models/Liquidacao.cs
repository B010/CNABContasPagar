using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Models
{
    public class Liquidacao
    {
        public string TipoPagamento { get; set; } // Fornecedores - 20
        public string FormaPagamento { get; set; }
        public string BancoFavorecido { get; set; }
        public string AgenciaFavorecido { get; set; }
        public string DigitoAgenciaFavorecido { get; set; }
        public string ContaFavorecido { get; set; }
        public string DacFavorecido { get; set; }
        public string NomeFavorecido { get; set; }
        public string NossoNumero { get; set; } // Não tem para Arquivo Remessa
        public DateTime DataPagamento { get; set; }
        public decimal ValorPagamento { get; set; }
        public string InscricaoEmpresa { get; set; }
        public string CpfCnpjFavorecido { get; set; }
        public string Documento { get; set; }
        public string CodigoBarras { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal Multa { get; set; }
        public decimal Mora { get; set; }
        public DateTime DataVencimento { get; set; }
        public string EnderecoFavorecido { get; set; }
        public string BairroFavorecido { get; set; }
        public string CidadeFavorecido { get; set; }
        public string CepFavorecido { get; set; }
        public string EstadoFavorecido { get; set; }
    }
}