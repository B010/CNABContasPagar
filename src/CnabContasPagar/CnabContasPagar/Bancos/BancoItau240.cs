using CnabContasPagar.Models;
using CnabContasPagar.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Bancos
{
    public class BancoItau240
    {
        private int codigoLote = 1;
        private int codigoDetalhe = 1;

        public BancoItau240(Opcoes opcoes)
        {
            Opcoes = opcoes;
        }

        public Opcoes Opcoes { get; set; }

        public string MontarArquivo(List<Liquidacao> liquidacoes)
        {
            codigoLote = 0;
            codigoDetalhe = 0;

            var b = new StringBuilder();

            HeaderArquivo(b);

            foreach(var liq in liquidacoes)
            {
                DetalhePagamentoComum(b, liq);
            }     

            TrailerArquivo(b);

            return b.ToString();
        }

        public void HeaderArquivo(StringBuilder b)
        {
            b.Append("34100000"); //01-08
            b.Append(new string(' ', 6)); //09-14
            b.Append("081"); //15-17
            b.Append('2'); //18-18 (1-CPF, 2-CNPJ)
            b.AppendNumero(14, Opcoes.CnpjPagador); //29-32
            b.Append(new string(' ', 20)); //33-52
            b.AppendNumero(5, Opcoes.NumeroAgencia); //53-57
            b.Append(' '); //58-58
            b.AppendNumero(12, Opcoes.NumeroContaCorrente); //59-70
            b.Append(' '); //71-71
            b.Append(Opcoes.DAC); //72-72
            b.AppendTexto(30, Opcoes.RazaoSocial); //73-102
            b.AppendTexto(30, Opcoes.NomeBanco); //103-132
            b.Append(new string(' ', 10)); //133-142
            b.Append('1'); //143-143 (1-REMESSSA, 2-RETORNO)
            b.AppendData(DateTime.Now); //144-151
            b.AppendData(DateTime.Now, "hhmmss"); //152-157
            b.AppendNumero(9, 0); //158-166
            b.AppendNumero(5, 0); //167-171 VERIFICAR DENSIDADE
            b.Append(new string(' ', 69)); //172-240
            b.Append(Environment.NewLine);
        }

        public void DetalhePagamentoComum(StringBuilder b, Liquidacao liquidacao)
        {
            HeaderDetalheComum(b, liquidacao);
            DetalheA(b, liquidacao);
            TrailerDetalheComum(b, liquidacao);
        }
        public void HeaderDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341"); //01-03
            b.AppendNumero(4, ++codigoLote); //04-07
            b.Append('1'); //08-08
            b.Append('C'); //09-09 (C=Credito)
            b.Append("01"); //10-11 TIPO DE PAGTO
            b.AppendNumero(2,liquidacao.FormaPagamento); //12-13 FORMA DE PAGAMENTO
            b.Append("040"); //14-16
            b.Append(' '); //17-17
            b.Append('2'); //18-18
            b.AppendNumero(14, Opcoes.CnpjPagador); //19-32
            b.Append("1707"); //33-36
            b.Append(new string(' ', 16)); //36-52
            b.AppendNumero(5, Opcoes.NumeroAgencia); //53-57
            b.Append(' '); //58-58
            b.AppendNumero(12, Opcoes.NumeroContaCorrente); //59-70
            b.Append(' '); //71-71
            b.Append(Opcoes.DAC); //72-72
            b.AppendTexto(30, Opcoes.RazaoSocial); //73-102
            b.Append(new string (' ', 30)); //103-132 FINALIDADE DO LOTE 
            b.Append(new string(' ', 10)); //133-142 HISTÓRICO DE C/C
            b.AppendTexto(30, Opcoes.EnderecoPagador); //143-172
            b.AppendNumero(5, Opcoes.Numero); //173-177
            b.Append(new string(' ', 15));  //178-192
            b.AppendTexto(20, Opcoes.Cidade); //193-212
            b.AppendNumero(8, Opcoes.Cep); //213-220
            b.AppendTexto(2, Opcoes.UF); //221-222
            b.Append(new string(' ', 8)); //223-230
            b.AppendNumero(10, 0); //231-240
            b.Append(Environment.NewLine);
        }

        public void DetalheA(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341"); //01-03
            b.AppendNumero(4, codigoLote); //04-07
            b.Append('3');
            b.AppendNumero(5, ++codigoDetalhe);
            b.Append('A');
            b.Append("000"); //TIPO DE MOVIMENTO
            b.Append("888");
            b.AppendNumero(3, liquidacao.BancoFavorecido);
            b.AppendTexto(20, FazerAgenciaContaFavorecido(liquidacao));
            b.AppendTexto(30, liquidacao.NomeFavorecido);
            b.AppendTexto(20, liquidacao.NossoNumero);
            b.AppendData(liquidacao.DataPagamento);
            b.Append("REA");
            b.Append(new string('0', 8)); //IDENTIFICAÇÃO DA INSTITUIÇÃO PARA O SPB
            b.AppendNumero(7, 0);
            b.AppendDinheiro(15, liquidacao.ValorPagamento);
            b.AppendTexto(20, " ");
            b.AppendNumero(8, 0);
            b.AppendNumero(15, 0);
            b.Append(new string(' ', 18));
            b.Append(new string(' ', 2));
            b.Append(new string('0', 6));
            b.AppendNumero(14, liquidacao.CpfCnpjFavorecido);
            b.Append(new string(' ', 2));
            b.Append(new string(' ', 5));
            b.Append(new string(' ', 5));
            b.Append('0');
            b.Append(new string (' ', 10));
            b.Append(Environment.NewLine);
        }

        public void TrailerDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341");
            b.Append(codigoLote);
            b.Append('5');
            b.Append(new string(' ', 9));
            b.AppendNumero(6,codigoDetalhe);
            b.AppendDinheiro(16, liquidacao.ValorPagamento);
            b.Append(new string('0', 18));
            b.Append(new string(' ', 171));
            b.Append(new string(' ', 10));
            b.Append(Environment.NewLine);
        }

        public void TrailerArquivo(StringBuilder b)
        {
            b.Append("341");
            b.Append("9999");
            b.Append('9');
            b.Append(new string(' ', 9));
            b.AppendNumero(6,codigoLote);
            b.AppendNumero(6,codigoDetalhe);
            b.Append(new string(' ', 211));
            b.Append(Environment.NewLine);
        }

        private string FazerAgenciaContaFavorecido(Liquidacao liquidacao)
        {
            var texto = new StringBuilder();

            if (liquidacao.BancoFavorecido == "341" || liquidacao.BancoFavorecido == "409")
            {
                texto.Append('0');
                texto.AppendNumero(4,liquidacao.AgenciaFavorecido);
                texto.Append(' ');
                texto.Append(new string('0', 6));

                if(liquidacao.FormaPagamento == "02" || liquidacao.FormaPagamento == "10")
                {
                    texto.AppendNumero(6, liquidacao.ContaFavorecido);
                    texto.Append(' ');
                    if (liquidacao.DacFavorecido.Length > 1)
                    {
                        texto.AppendNumero(2, liquidacao.DacFavorecido);
                    }
                    else
                    {
                        texto.Append(' ');
                        texto.Append(liquidacao.DacFavorecido);
                    }
                }
                else
                {
                    texto.Append(new string('0', 6));
                    texto.Append(' ');
                    texto.Append('0');
                }
            }
            else
            {
                texto.AppendNumero(5, liquidacao.AgenciaFavorecido);
                texto.Append(' ');
                texto.AppendNumero(12, liquidacao.ContaFavorecido);
                if(liquidacao.DacFavorecido.Length > 1)
                {
                    texto.AppendNumero(2, liquidacao.DacFavorecido);
                }
                else
                {
                    texto.Append(' ');
                    texto.Append(liquidacao.DacFavorecido);
                }
                
            }
            return (texto.ToString());
        }
    }
}
