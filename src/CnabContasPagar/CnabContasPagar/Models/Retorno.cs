using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Models
{
    public class Retorno
    {
        public string CodigoBanco { get; set; }
        public DateTime DataGravacao { get; set; }
        public int CodigoAviso { get; set; }
        public string Aviso { get; set; }

        public List<ItemRetorno> Itens { get; set; } = new List<ItemRetorno>();
    }

    public class ItemRetorno
    {
        public int NumeroLinha { get; set; }
        public int IdTitulo { get; set; }
        public int Parcela { get; set; }
        public int IdBanco { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public DateTime DataPrevista { get; set; }
        public string NomeFornecedor { get; set; }
        public int Situacao { get; set; }
        public string TextoSituacao { get; set; }
    }
}