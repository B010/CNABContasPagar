using CnabContasPagar.Models;
using CnabContasPagar.Util;
using CNABContasPagar.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Bancos
{
    public class BancoItau240 : IBanco
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

            foreach (var liq in liquidacoes)
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
            b.AppendNumero(5, 0); //167-171 Unidade de DENSIDADE
            b.Append(new string(' ', 69)); //172-240
            b.Append(Environment.NewLine);
        }

        public void DetalhePagamentoComum(StringBuilder b, Liquidacao liquidacao)
        {
            if (liquidacao.FormaPagamento != "30" && liquidacao.FormaPagamento != "31")
            {

                HeaderDetalheComum(b, liquidacao);
                DetalheA(b, liquidacao);
                TrailerDetalheComum(b, liquidacao);
            }
            else
            {
                HeaderBoleto(b, liquidacao);
                DetalheBloqueto(b, liquidacao);
                DetalheBoleto(b, liquidacao);
                TrailerBoleto(b, liquidacao);
            }
        }

        public void HeaderDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341"); //01-03
            b.AppendNumero(4, ++codigoLote); //04-07
            b.Append('1'); //08-08
            b.Append('C'); //09-09 (C=Credito)
            b.AppendNumero(2, 20); //10-11 TIPO DE PAGTO
            b.AppendNumero(2, liquidacao.FormaPagamento); //12-13 FORMA DE PAGAMENTO
            b.Append("040"); //14-16
            b.Append(' '); //17-17
            b.Append('2'); //18-18
            b.AppendNumero(14, Opcoes.CnpjPagador); //19-32
            b.Append("1707"); //33-36
            b.Append(new string(' ', 16)); //37-52
            b.AppendNumero(5, Opcoes.NumeroAgencia); //53-57
            b.Append(' '); //58-58
            b.AppendNumero(12, Opcoes.NumeroContaCorrente); //59-70
            b.Append(' '); //71-71
            b.Append(Opcoes.DAC); //72-72
            b.AppendTexto(30, Opcoes.RazaoSocial); //73-102
            b.Append(new string(' ', 30)); //103-132 FINALIDADE DO LOTE
            b.Append(new string(' ', 10)); //133-142 HISTÓRICO DE C/C
            b.AppendTexto(30, Opcoes.EnderecoPagador); //143-172
            b.AppendNumero(5, 0); //173-177 NUMERO
            b.Append(new string(' ', 15));  //178-192
            b.AppendTexto(20, Opcoes.Cidade); //193-212
            b.AppendNumero(8, Opcoes.Cep); //213-220
            b.AppendTexto(2, Opcoes.UF); //221-222
            b.Append(new string(' ', 8)); //223-230
            b.Append(new string(' ', 10)); //231-240
            b.Append(Environment.NewLine);
        }

        public void DetalheA(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341"); //01-03
            b.AppendNumero(4, codigoLote); //04-07
            b.Append('3');
            b.AppendNumero(5, ++codigoDetalhe);
            b.Append('A');
            b.Append("000"); //TIPO DE MOVIMENTO (000 = Inclusão de pagamento)
            b.AppendNumero(3, 0); // CÓDIGO DA CÂMARA CENTRALIZADORA = "888" (Somente para forma de pagamento TED para Corretora)
            b.AppendNumero(3, liquidacao.BancoFavorecido);
            b.AppendTexto(20, FazerAgenciaContaFavorecido(liquidacao));
            b.AppendTexto(30, liquidacao.NomeFavorecido);
            b.AppendTexto(20, liquidacao.Documento); // Seu Numero
            b.AppendData(liquidacao.DataPagamento);
            b.Append("REA");
            b.AppendNumero(8, 0); //IDENTIFICAÇÃO DA INSTITUIÇÃO PARA O SPB = "60701190" (Somente para forma de pagamento TED para Corretora)
            b.AppendNumero(7, 0);
            b.AppendDinheiro(15, liquidacao.ValorPagamento);
            b.AppendTexto(20, " "); // Nosso Numero
            b.AppendNumero(8, 0);
            b.AppendNumero(15, 0);
            b.Append(new string(' ', 18));
            b.Append(new string(' ', 2));
            b.AppendNumero(6, 0);
            b.AppendNumero(14, liquidacao.CpfCnpjFavorecido);

            if (liquidacao.FormaPagamento == "03")
            {
                b.AppendNumero(2, 07); // Finalidade do Doc (Pgto Fornecedores / Honorários)
            }
            else
            {
                b.Append(new string(' ', 2));
            }

            b.AppendTexto(5, "00005"); // Finalidade da TED (Pgto Fornecedores)
            b.Append(new string(' ', 5));
            b.Append('0');
            b.Append(new string(' ', 10));
            b.Append(Environment.NewLine);
        }

        public void TrailerDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341");
            b.AppendNumero(4, codigoLote);
            b.Append('5');
            b.Append(new string(' ', 9));
            b.AppendNumero(6, codigoDetalhe);
            b.AppendDinheiro(18, liquidacao.ValorPagamento);
            b.Append(new string('0', 18));
            b.Append(new string(' ', 171));
            b.Append(new string(' ', 10));
            b.Append(Environment.NewLine);
        }

        public void HeaderBoleto(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341"); //01-03
            b.AppendNumero(4, ++codigoLote); //04-07
            b.Append('1'); //08-08
            b.Append('C'); //09-09 (C=Credito)
            b.AppendNumero(2, 20); //10-11 TIPO DE PAGTO
            b.AppendNumero(2, liquidacao.FormaPagamento); //12-13 FORMA DE PAGAMENTO
            b.Append("030"); //14-16
            b.Append(' '); //17-17
            b.Append('2'); //18-18
            b.AppendNumero(14, Opcoes.CnpjPagador); //19-32
            b.Append(new string(' ', 20)); //033-052
            b.AppendNumero(5, Opcoes.NumeroAgencia); //53-57
            b.Append(' '); //58-58
            b.AppendNumero(12, Opcoes.NumeroContaCorrente); //59-70
            b.Append(' '); //71-71
            b.Append(Opcoes.DAC); //72-72
            b.AppendTexto(30, Opcoes.RazaoSocial); //73-102
            b.Append(new string(' ', 30)); //103-132 FINALIDADE DO LOTE
            b.Append(new string(' ', 10)); //133-142 HISTÓRICO DE C/C
            b.AppendTexto(30, Opcoes.EnderecoPagador); //143-172
            b.AppendNumero(5, 0); //173-177 NUMERO
            b.Append(new string(' ', 15));  //178-192
            b.AppendTexto(20, Opcoes.Cidade); //193-212
            b.AppendNumero(8, Opcoes.Cep); //213-220
            b.AppendTexto(2, Opcoes.UF); //221-222
            b.Append(new string(' ', 8)); //223-230
            b.Append(new string(' ', 10)); //231-240
            b.Append(Environment.NewLine);
        }

        public void DetalheBloqueto(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341"); //01-03
            b.AppendNumero(4, codigoLote); //04-07
            b.Append('3');
            b.AppendNumero(5, ++codigoDetalhe);
            b.Append('J');
            b.Append("000"); //TIPO DE MOVIMENTO (000 = Inclusão de pagamento)
            b.AppendTexto(44, FormataCodigoBarras(liquidacao));
            b.AppendTexto(30, liquidacao.NomeFavorecido);
            b.AppendData(liquidacao.DataVencimento);
            b.AppendDinheiro(15, liquidacao.ValorPagamento);
            b.AppendDinheiro(15, liquidacao.ValorDesconto);
            b.AppendTexto(15, CalcularMultaMora(liquidacao));
            b.AppendData(liquidacao.DataPagamento);
            b.AppendDinheiro(15, liquidacao.ValorPagamento);
            b.Append(new string('0', 15));
            b.AppendTexto(20, liquidacao.Documento); // Seu Numero
            b.Append(new string(' ', 13));
            b.Append(new string(' ', 15));
            b.Append(new string(' ', 10));
            b.Append(Environment.NewLine);
        }

        public void DetalheBoleto(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341"); //01-03
            b.AppendNumero(4, codigoLote); //04-07
            b.Append('3');
            b.AppendNumero(5, ++codigoDetalhe);
            b.Append('J');
            b.Append("000"); //TIPO DE MOVIMENTO (000 = Inclusão de pagamento)
            b.Append("52"); // Identificação do Registro Opcional
            b.Append('2');
            b.Append('0');
            b.AppendNumero(14, Opcoes.CnpjPagador);
            b.AppendTexto(40, Opcoes.RazaoSocial);
            b.AppendTexto(2, liquidacao.InscricaoEmpresa); // Beneficiário
            b.AppendTexto(15, CnpjOuCpf(liquidacao));
            b.AppendTexto(40, liquidacao.NomeFavorecido);
            b.AppendTexto(2, liquidacao.InscricaoEmpresa); // Sacador Avalista
            b.AppendTexto(15, CnpjOuCpf(liquidacao));
            b.AppendTexto(40, liquidacao.NomeFavorecido);
            b.Append(new string(' ', 53));
            b.Append(Environment.NewLine);
        }

        public void TrailerBoleto(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("341");
            b.AppendNumero(4, codigoLote);
            b.Append('5');
            b.Append(new string(' ', 9));
            b.AppendNumero(6, codigoDetalhe);
            b.AppendDinheiro(18, liquidacao.ValorPagamento);
            b.AppendNumero(18, 0);
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
            b.AppendNumero(6, codigoLote);
            b.AppendNumero(6, codigoDetalhe);
            b.Append(new string(' ', 211));
            b.Append(Environment.NewLine);
        }

        private string FazerAgenciaContaFavorecido(Liquidacao liquidacao)
        {
            var texto = new StringBuilder();

            if (liquidacao.BancoFavorecido == "341" || liquidacao.BancoFavorecido == "409")
            {
                texto.Append('0');
                texto.AppendNumero(4, liquidacao.AgenciaFavorecido);
                texto.Append(' ');
                texto.Append(new string('0', 6));

                if (liquidacao.FormaPagamento == "02" || liquidacao.FormaPagamento == "10")
                {
                    texto.Append(new string('0', 6));
                    texto.Append(' ');
                    texto.Append('0');
                }
                else
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
            }
            else
            {
                texto.AppendNumero(5, liquidacao.AgenciaFavorecido);
                texto.Append(' ');
                texto.AppendNumero(12, liquidacao.ContaFavorecido);
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

            return (texto.ToString());
        }

        public string ValidaPagto(string formaPagto, string numeroBanco)
        {
            var x = "";

            if (formaPagto == "01" && (numeroBanco != "341" && numeroBanco != "409"))
            {
                x = "Para pagto via Crédito em Conta, é necessário que todos os Fornecedores selecionados tenham conta no banco Itaú!";
            }

            return x;
        }

        private string CnpjOuCpf(Liquidacao liquidacao)
        {
            var texto = new StringBuilder();

            if (liquidacao.InscricaoEmpresa == "1")
            {
                texto.AppendTexto(11, liquidacao.CpfCnpjFavorecido);
                texto.Append(new string(' ', 4));
            }
            else
            {
                texto.Append('0');
                texto.AppendTexto(14, liquidacao.CpfCnpjFavorecido);
            }

            return (texto.ToString());
        }

        private string CalcularMultaMora(Liquidacao liquidacao)
        {
            var texto = new StringBuilder();

            var valorMultaMora = liquidacao.Multa + liquidacao.Mora;
            texto.AppendDinheiro(15, valorMultaMora);

            return (texto.ToString());
        }

        public string ExisteCodBarras(string formaPagto, string codBarras)
        {
            var x = "";

            if ((formaPagto == "30" || formaPagto == "31") && codBarras == "")
            {
                x = "Para pagto via Boleto, é necessário que todos os Títulos selecionados tenham Código de Barras informado";
            }

            return x;
        }
        
        private bool ValidaDvGeral(string codBarras)
        {
            bool retorno;
            int soma = 0, resto = 0, calculoDV = 0;

            char[] strs = new char[codBarras.Length];

            for (int i = codBarras.Length - 1; i >= 0; i--)
            {
                strs[i] = codBarras[i];
            }

            int[] cod = Array.ConvertAll(strs, c => (int)Char.GetNumericValue(c));

            if (codBarras.Length == 44)
            {
                soma += cod[43] * 2;
                soma += cod[42] * 3;
                soma += cod[41] * 4;
                soma += cod[40] * 5;
                soma += cod[39] * 6;
                soma += cod[38] * 7;
                soma += cod[37] * 8;
                soma += cod[36] * 9;

                soma += cod[35] * 2;
                soma += cod[34] * 3;
                soma += cod[33] * 4;
                soma += cod[32] * 5;
                soma += cod[31] * 6;
                soma += cod[30] * 7;
                soma += cod[29] * 8;
                soma += cod[28] * 9;

                soma += cod[27] * 2;
                soma += cod[26] * 3;
                soma += cod[25] * 4;
                soma += cod[24] * 5;
                soma += cod[23] * 6;
                soma += cod[22] * 7;
                soma += cod[21] * 8;
                soma += cod[20] * 9;

                soma += cod[19] * 2;
                soma += cod[18] * 3;
                soma += cod[17] * 4;
                soma += cod[16] * 5;
                soma += cod[15] * 6;
                soma += cod[14] * 7;
                soma += cod[13] * 8;
                soma += cod[12] * 9;

                soma += cod[11] * 2;
                soma += cod[10] * 3;
                soma += cod[9] * 4;
                soma += cod[8] * 5;
                soma += cod[7] * 6;
                soma += cod[6] * 7;
                soma += cod[5] * 8;
                soma += cod[3] * 9;

                soma += cod[2] * 2;
                soma += cod[1] * 3;
                soma += cod[0] * 4;
                
                resto += soma % 11;
                
                calculoDV += 11 - resto;

                if (cod[4] == calculoDV)
                    retorno = true;
                else
                    retorno = false;
            }
            else
            {
                soma += cod[46] * 2;
                soma += cod[45] * 3;
                soma += cod[44] * 4;
                soma += cod[43] * 5;
                soma += cod[42] * 6;
                soma += cod[41] * 7;
                soma += cod[40] * 8;
                soma += cod[39] * 9;

                soma += cod[38] * 2;
                soma += cod[37] * 3;
                soma += cod[36] * 4;
                soma += cod[35] * 5;
                soma += cod[34] * 6;
                soma += cod[33] * 7;
                soma += cod[30] * 8;
                soma += cod[29] * 9;

                soma += cod[28] * 2;
                soma += cod[27] * 3;
                soma += cod[26] * 4;
                soma += cod[25] * 5;
                soma += cod[24] * 6;
                soma += cod[23] * 7;
                soma += cod[22] * 8;
                soma += cod[21] * 9;

                soma += cod[19] * 2;
                soma += cod[18] * 3;
                soma += cod[17] * 4;
                soma += cod[16] * 5;
                soma += cod[15] * 6;
                soma += cod[14] * 7;
                soma += cod[13] * 8;
                soma += cod[12] * 9;

                soma += cod[11] * 2;
                soma += cod[10] * 3;
                soma += cod[8] * 4;
                soma += cod[7] * 5;
                soma += cod[6] * 6;
                soma += cod[5] * 7;
                soma += cod[4] * 8;
                soma += cod[3] * 9;

                soma += cod[2] * 2;
                soma += cod[1] * 3;
                soma += cod[0] * 4;

                resto += soma % 11;

                calculoDV += 11 - resto;

                if (cod[32] == calculoDV)
                    retorno = true;
                else
                    retorno = false;
            }

            return retorno;
        }

        private bool ValidaDvUnitario(string codBarras)
        {
            bool retorno;

            char[] strs = new char[codBarras.Length];

            for (int i = codBarras.Length - 1; i >= 0; i--)
            {
                strs[i] = codBarras[i];
            }

            int[] cod = Array.ConvertAll(strs, c => (int)Char.GetNumericValue(c));

            int somaCampo1 = 0, somaCampo2 = 0, somaCampo3 = 0, restoCampo1 = 0, restoCampo2 = 0, restoCampo3 = 0, dvCampo1 = 0, dvCampo2 = 0, dvCampo3 = 0;

            somaCampo3 += cod[30] * 2;
            somaCampo3 += cod[29] * 1;
            somaCampo3 += cod[28] * 2;
            somaCampo3 += cod[27] * 1;
            somaCampo3 += cod[26] * 2;
            somaCampo3 += cod[25] * 1;
            somaCampo3 += cod[24] * 2;
            somaCampo3 += cod[23] * 1;
            somaCampo3 += cod[22] * 2;
            somaCampo3 += cod[21] * 1;

            somaCampo2 += cod[19] * 2;
            somaCampo2 += cod[18] * 1;
            somaCampo2 += cod[17] * 2;
            somaCampo2 += cod[16] * 1;
            somaCampo2 += cod[15] * 2;
            somaCampo2 += cod[14] * 1;
            somaCampo2 += cod[13] * 2;
            somaCampo2 += cod[12] * 1;
            somaCampo2 += cod[11] * 2;
            somaCampo2 += cod[10] * 1;

            somaCampo1 += cod[8] * 2;
            somaCampo1 += cod[7] * 1;
            somaCampo1 += cod[6] * 2;
            somaCampo1 += cod[5] * 1;
            somaCampo1 += cod[4] * 2;
            somaCampo1 += cod[3] * 1;
            somaCampo1 += cod[2] * 2;
            somaCampo1 += cod[1] * 1;
            somaCampo1 += cod[0] * 2;

            restoCampo3 += somaCampo3 % 10;
            restoCampo2 += somaCampo2 % 10;
            restoCampo1 += somaCampo1 % 10;

            dvCampo3 += 10 - restoCampo3;
            dvCampo2 += 10 - restoCampo2;
            dvCampo1 += 10 - restoCampo1;

            if (dvCampo3 == 10)
                dvCampo3 = 0;
            if (dvCampo2 == 10)
                dvCampo2 = 0;
            if (dvCampo1 == 10)
                dvCampo1 = 0;

            if (cod[31] == dvCampo3 && cod[20] == dvCampo2 && cod[9] == dvCampo1)
                retorno = true;
            else
                retorno = false;

            return retorno;
        }

        public string ValidaCodBarras(string codBarras)
        {
            string x;
            bool valido;

            if (codBarras.Length == 44)
                valido = ValidaDvGeral(codBarras);
            if (codBarras.Length == 47)
            {
                bool dvGeral, dvUnitario;

                dvGeral = ValidaDvGeral(codBarras);
                dvUnitario = ValidaDvUnitario(codBarras);

                if (dvGeral == true && dvUnitario == true)
                    valido = true;
                else
                    valido = false;
            }
            else
            {
                valido = false;
            }    

            if (valido)
                x = "";
            else
                x = "false";

            return x;
        }

        private string FormataCodigoBarras(Liquidacao liquidacao)
        {
            var texto = new StringBuilder();

            string codBanco, codMoeda, dv, fatVencto, valor, campoLivre;

            if (liquidacao.CodigoBarras.Length == 44)
            {
                codBanco = liquidacao.CodigoBarras.Substring(0, 3);
                codMoeda = liquidacao.CodigoBarras.Substring(3, 1);
                dv = liquidacao.CodigoBarras.Substring(4, 1);
                fatVencto = liquidacao.CodigoBarras.Substring(5, 4);
                valor = liquidacao.CodigoBarras.Substring(6, 10);
                campoLivre = liquidacao.CodigoBarras.Substring(7, 25);
            }
            else
            {
                string campo1, campo2, campo3;

                codBanco = liquidacao.CodigoBarras.Substring(0, 3);
                codMoeda = liquidacao.CodigoBarras.Substring(3, 1);

                campo1 = liquidacao.CodigoBarras.Substring(4, 5);
                campo2 = liquidacao.CodigoBarras.Substring(10, 10);
                campo3 = liquidacao.CodigoBarras.Substring(21, 10);
                campoLivre = campo1 + campo2 + campo3;

                dv = liquidacao.CodigoBarras.Substring(32, 1);
                fatVencto = liquidacao.CodigoBarras.Substring(33, 4);
                valor = liquidacao.CodigoBarras.Substring(37, 10);
            }

            texto.AppendTexto(3, codBanco);
            texto.AppendTexto(1, codMoeda);
            texto.AppendTexto(1, dv);
            texto.AppendTexto(4, fatVencto);
            texto.AppendTexto(10, valor);
            texto.AppendTexto(25, campoLivre);

            return (texto.ToString());
        }
    }
}
