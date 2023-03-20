using CnabContasPagar.Models;
using CnabContasPagar.Util;
using CNABContasPagar.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CnabContasPagar.Retornos
{
    public class Retornos : IRetorno
    {
        public Retorno LerArquivo(Stream arquivo)
        {
            var retorno = new Retorno();

            // grava a tabela temporaria
            var Sr = new StreamReader(arquivo);
            string S;
            string NumeroBanco = "";

            while (!Sr.EndOfStream)
            {
                S = Sr.ReadLine();

                if (S.Substring(0, 1) == "0")
                {
                    if (S.Length == 500)
                    {
                        NumeroBanco = "237";
                    }

                    switch (NumeroBanco)
                    {
                        case "237":
                            {
                                retorno.CodigoBanco = "237";
                                retorno.CodigoAviso = Convert.ToInt32(S.Substring(105, 1));
                                retorno.Aviso = RetornaTextoSituacaoHeader("237", Convert.ToInt32(S.Substring(105, 1)));
                                retorno.DataGravacao = new DateTime(Convert.ToInt32(S.Substring(78, 4)), Convert.ToInt32(S.Substring(82, 2)), Convert.ToInt32(S.Substring(84, 2)));
                                break;
                            }
                        default:
                            {
                                retorno.CodigoBanco = "0";
                                retorno.CodigoAviso = 0;
                                retorno.DataGravacao = DateTime.Now;
                                break;
                            }
                    }
                }

                if (S.Substring(0, 1) == "1")
                {
                    if (PodeProsseguir(retorno.CodigoBanco, S))
                    {
                        retorno.Itens.Add(new ItemRetorno
                        {
                            NumeroLinha = RetornaLinha(retorno.CodigoBanco, S),
                            IdTitulo = RetornaTitulo(retorno.CodigoBanco, S),
                            Parcela = RetornaParcela(retorno.CodigoBanco, S),
                            IdBanco = RetornaIdBanco(retorno.CodigoBanco, S),
                            Valor = RetornaValorPago(retorno.CodigoBanco, S),
                            Situacao = RetornaSituacao(retorno.CodigoBanco, S),
                            TextoSituacao = RetornaTextoSituacao(retorno.CodigoBanco, retorno.CodigoAviso, RetornaSituacao(retorno.CodigoBanco, S)),
                            DataPrevista = RetornaDataPrevista(retorno.CodigoBanco, S),
                            NomeFornecedor = RetornaNomeFornecedor(retorno.CodigoBanco, S),
                            ValorAcrescimo = RetornaValorAcrescimo(retorno.CodigoBanco, S),
                            ValorDesconto = RetornaValorDesconto(retorno.CodigoBanco, S),
                            ValorTitulo = RetornaValorTitulo(retorno.CodigoBanco, S),
                            Erro = RetornaValorErro(retorno.CodigoBanco, S)
                        });
                    }
                }
            }

            return retorno;
        }

        public int RetornaLinha(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return Convert.ToInt32(linha.Substring(494, 6));

            return 0;
        }

        public bool PodeProsseguir(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                if (linha.Substring(150, 15).Contains("A"))
                    return true;

            return false;
        }

        public int RetornaTitulo(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return Convert.ToInt32(linha.Substring(150, 15).Split('A')[0].Trim());

            return 0;
        }

        public int RetornaParcela(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return Convert.ToInt32(linha.Substring(150, 15).Split('A')[1].Trim());

            return 0;
        }

        public int RetornaIdBanco(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return Convert.ToInt32(linha.Substring(150, 15).Split('A')[2].Trim());

            return 0;
        }

        public decimal RetornaValorPago(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return (Convert.ToDecimal(linha.Substring(204, 15)) / 100);

            return 0;
        }

        public decimal RetornaValorDesconto(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return (Convert.ToDecimal(linha.Substring(219, 15)) / 100);

            return 0;
        }

        public decimal RetornaValorTitulo(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return (Convert.ToDecimal(linha.Substring(194, 10)) / 100);

            return 0;
        }

        public string RetornaValorErro(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return linha.Substring(279, 10);

            return "";
        }

        public decimal RetornaValorAcrescimo(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return (Convert.ToDecimal(linha.Substring(234, 15)) / 100);

            return 0;
        }

        public DateTime RetornaDataPrevista(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return new DateTime(Convert.ToInt32(linha.Substring(265, 4)), Convert.ToInt32(linha.Substring(269, 2)), Convert.ToInt32(linha.Substring(271, 2)));

            return new DateTime();
        }

        public string RetornaNomeFornecedor(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
                return linha.Substring(17, 30);

            return "";
        }

        public int RetornaSituacao(string numeroBanco, string linha)
        {
            if (numeroBanco == "237")
            {
                return Convert.ToInt32(linha.Substring(276, 2));
            }
            return 0;
        }

        public string RetornaTextoSituacaoHeader(string numeroBanco, int situacaoHeader)
        {
            if (numeroBanco == "237")
            {
                if (situacaoHeader == 1)
                {
                    return "Rastreamento da Cobrança Bradesco / Rastreamento DDA / Cheque estornado e Doc COMPE devolvido";
                }
                else if (situacaoHeader == 2)
                {
                    return "Confirmação de Agendamento / Inconsistência";
                }
                else if (situacaoHeader == 3)
                {
                    return "Confirmação de Pagamento / Pagamento não Efetuado";
                }
                return "";
            }

            return "";
        }

        public string RetornaTextoSituacao(string numeroBanco, int situacaoHeader, int situacaoLinha)
        {
            if (numeroBanco == "237")
            {
                if (situacaoHeader == 1)
                {
                    switch (situacaoLinha)
                    {
                        case 1:
                            return "NÃO PAGO";
                        case 2:
                            return "PAGO";
                        case 5:
                            return "BAIXA COBR SEM PAGAMENTO";
                        case 6:
                            return "BAIXA COBR COM PAGAMENTO";
                        case 7:
                            return "COM INST DE PROTESTO";
                        case 8:
                            return "TRASF PARA CARTÓRIO";
                        case 9:
                            return "BAIXADO PELO DESCONTO";
                        case 11:
                            return "CHEQUE OP ESTORNADO";
                    }
                }
                else if (situacaoHeader == 2)
                {
                    if (situacaoLinha == 1)
                        return "NÃO PAGO";
                }
                else if (situacaoHeader == 3)
                {
                    switch (situacaoLinha)
                    {
                        case 1:
                            return "NÃO PAGO";
                        case 2:
                            return "PAGO";
                        case 22:
                            return "CHEQUE O.P. EMITIDO";
                    }
                }
                return "";
            }

            return "";
        }
    }
}