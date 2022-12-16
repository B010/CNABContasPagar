using CnabContasPagar.Models;
using CnabContasPagar.Util;
using CNABContasPagar.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Bancos
{
    public class BancoBradesco240 : IBanco
    {
        private int codigoLote = 1;
        private int codigoDetalhe = 1;
        private int qtdeArquivo = 1;
        private int qtdeLinhasArquivo = 1;
        private int qtdeLinhasLote = 1;

        public BancoBradesco240(Opcoes opcoes)
        {
            Opcoes = opcoes;
        }

        public Opcoes Opcoes { get; set; }

        public string MontarArquivo(List<Liquidacao> liquidacoes)
        {
            codigoLote = 0;
            qtdeArquivo = 0;
            qtdeLinhasArquivo = 0;
            qtdeLinhasLote = 0;

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
            ++qtdeArquivo; //NAO APAGAR
            ++qtdeLinhasArquivo; //NAO APAGAR

            b.Append("23700000"); //01-08
            b.Append(new string(' ', 9)); //09-17
            b.Append('2'); //18-18 (1-CPF, 2-CNPJ)
            b.AppendNumero(14, Opcoes.CnpjPagador); //19-32

            b.Append(new string(' ', 20)); //33-52                  QUAL O CODIGO ADOTADO PELO BANCO?

            b.AppendNumero(5, Opcoes.NumeroAgencia); //53-57
            b.AppendNumero(1, Opcoes.DigitoAgencia); //58-58
            b.AppendNumero(12, Opcoes.NumeroContaCorrente); //59-70
            b.AppendNumero(1, RetornaDigitoCC(1, Opcoes.DAC)); //71-71
            b.AppendNumero(1, RetornaDigitoCC(2, Opcoes.DAC)); //72-72
            b.AppendTexto(30, Opcoes.RazaoSocial); //73-102
            b.AppendTexto(30, Opcoes.NomeBanco); //103-132
            b.Append(new string(' ', 10)); //133-142
            b.Append('1'); //143-143 (1-REMESSSA, 2-RETORNO)
            b.AppendData(DateTime.Now); //144-151
            b.AppendData(DateTime.Now, "hhmmss"); //152-157

            b.AppendNumero(6, qtdeArquivo); //158-163               PQ NUMERO SEQUENCIAL DE ARQUIVO?

            b.Append("080"); //164-166

            b.Append("01600"); //167-171 DENSIDADE DE GRAVACAO      -1600 BPI ou 6250 BPI - QUAL CODIGO BPI INFORMAR? PQ 5 POSICOES SE SAO 4 DIGITOS NO CODIGO?

            b.Append(new string(' ', 69)); //172-240
            b.Append(Environment.NewLine);
        }

        public void DetalhePagamentoComum(StringBuilder b, Liquidacao liquidacao)
        {
            if (liquidacao.FormaPagamento != "30" && liquidacao.FormaPagamento != "31")
            {
                HeaderDetalheComum(b, liquidacao);

                DetalheA(b, liquidacao);
                DetalheB(b, liquidacao);

                TrailerDetalheComum(b, liquidacao);
            }
            else
            {
                //HeaderBoleto(b, liquidacao);
                //DetalheBoleto(b, liquidacao);
                //DetalheBoletoOnline(b, liquidacao);
                //TrailerBoleto(b, liquidacao);
            }
        }

        public void HeaderDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {
            ++qtdeLinhasArquivo; //NAO APAGAR
            ++qtdeLinhasLote; //NAO APAGAR

            b.Append("237"); //01-03
            b.AppendNumero(4, ++codigoLote); //04-07                        MSM NUMERO PARA CADA LINHA DO LOTE OU ACRESCE A CONTAGEM A CADA LINHA?
            b.Append('1'); //08-08
            b.Append('C'); //09-09 (C=Credito)                              C -LANCAMENTO A CREDITO, R -ARQUIVO REMESSA
            b.AppendNumero(2, 20); //10-11 TIPO DO SERVICO

            b.AppendNumero(2, liquidacao.FormaPagamento); //12-13 FORMA DE LANCAMENTO
            
            b.Append("040"); //14-16
            b.Append(' '); //17-17
            b.Append('2'); //18-18
            b.AppendNumero(14, Opcoes.CnpjPagador); //19-32

            b.Append(new string(' ', 20)); //33-52                          QUAL O CODIGO ADOTADO PELO BANCO?

            b.AppendNumero(5, Opcoes.NumeroAgencia); //53-57
            b.AppendNumero(1, Opcoes.DigitoAgencia); //58-58
            b.AppendNumero(12, Opcoes.NumeroContaCorrente); //59-70
            b.AppendNumero(1, RetornaDigitoCC(1, Opcoes.DAC)); //71-71
            b.AppendNumero(1, RetornaDigitoCC(2, Opcoes.DAC)); //72-72
            b.AppendTexto(30, Opcoes.RazaoSocial); //73-102
            b.Append(new string(' ', 40)); //103-142 MENSAGEM
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
            ++qtdeLinhasArquivo; //NAO APAGAR
            ++qtdeLinhasLote; //NAO APAGAR

            codigoDetalhe = 0;  //NAO APAGAR

            b.Append("237"); //01-03
            b.AppendNumero(4, codigoLote); //04-07
            b.Append('3'); //08-08
            b.AppendNumero(5, ++codigoDetalhe); //09-13
            b.Append('A'); //14-14

            b.Append("0"); //15-15 TIPO DE MOVIMENTO                        0 -INCLUSAO OU 7 -LIQUIDACAO?

            b.Append("23"); //16-17 CODIGO DA INSTRUCAO PARA MOVIMENTO      00 -INCLUSAO DETALHE LIBERADO OU 23 -PGTO DIRETO FORNECEDOR?

            if (liquidacao.FormaPagamento == "03")
            {
                b.Append("700"); //18-20 CÓDIGO DA CÂMARA CENTRALIZADORA -CODIGO P/ DOC
            }
            else
            {
                b.Append("018"); //18-20 -CODIGO P/ TED
            }

            b.AppendNumero(3, liquidacao.BancoFavorecido); //21-23
            b.AppendNumero(5, liquidacao.AgenciaFavorecido); //24-28
            b.AppendNumero(1, liquidacao.DigitoAgenciaFavorecido); //29-29
            b.AppendNumero(12, liquidacao.ContaFavorecido); //30-41
            b.AppendNumero(1, RetornaDigitoCC(1, Opcoes.DAC)); //42-42
            b.AppendNumero(1, RetornaDigitoCC(2, Opcoes.DAC)); //43-43
            b.AppendTexto(30, liquidacao.NomeFavorecido); //44-73
            b.AppendNumero(20, liquidacao.Documento); //74-93 SEU NUMERO
            b.AppendData(liquidacao.DataPagamento); //94-101
            b.Append("BRL"); //102-104

            b.AppendNumero(10, 0); //105-119 QTDE DA MOEDA                                  O QUE INFORMAR AQUI?

            b.AppendDinheiro(13, liquidacao.ValorPagamento); //120-134
            b.Append(new string(' ', 20)); //135-154
            b.AppendNumero(21, 0); //155-177
            b.Append(new string(' ', 40)); //178-217 INFORMACAO 2

            b.Append("07"); //218-219 FINALIDADE DOC                                        SMP INFORMAR O DOC?
            b.AppendTexto(5, "00005"); //220-224 Finalidade da TED (Pgto Fornecedores)      SMP INFORMACAR O TED?

            b.Append(new string(' ', 5)); //225-229
            b.AppendNumero(1, 0); //230-230
            b.Append(new string(' ', 10)); //231-140
            b.Append(Environment.NewLine);
        }

        public void DetalheB(StringBuilder b, Liquidacao liquidacao)
        {
            ++qtdeLinhasArquivo; //NAO APAGAR
            ++qtdeLinhasLote; //NAO APAGAR

            b.Append("237"); //01-03
            b.AppendNumero(4, codigoLote); //04-07
            b.Append('3'); //08-08
            b.AppendNumero(5, ++codigoDetalhe); //09-13
            b.Append('B'); //14-14
            b.Append(new string(' ', 3)); //15-17
            b.AppendTexto(1, ChecaInscricaoEmp(liquidacao.CpfCnpjFavorecido)); //18-18 Beneficiário
            b.AppendTexto(14, liquidacao.CpfCnpjFavorecido); //19-32
            b.AppendTexto(30, liquidacao.EnderecoFavorecido); //33-62
            b.AppendNumero(5, 0); //63-67 Numero
            b.Append(new string(' ', 15)); //68-82 Complemento
            b.AppendTexto(15, liquidacao.BairroFavorecido); //83-97
            b.AppendTexto(20, liquidacao.CidadeFavorecido); //98-117
            b.AppendNumero(8, liquidacao.CepFavorecido); //118-125
            b.AppendTexto(2, liquidacao.EstadoFavorecido); //126-127
            b.AppendData(liquidacao.DataVencimento); //128-135
            b.AppendDinheiro(13, liquidacao.ValorPagamento); //136-150
            b.AppendNumero(13, 0); //151-165
            b.AppendDinheiro(13, liquidacao.ValorDesconto); //166-180
            b.AppendDinheiro(13, liquidacao.Mora); //181-195
            b.AppendDinheiro(13, liquidacao.Multa); //196-210
            b.AppendNumero(15, liquidacao.CpfCnpjFavorecido); //211-225
            b.Append(new string(' ', 15)); //226-240
            b.Append(Environment.NewLine);
        }

        public void TrailerDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {
            ++qtdeLinhasArquivo; //NAO APAGAR

            b.Append("237"); //01-03
            b.AppendNumero(4, codigoLote); //04-07
            b.Append('5'); //08-08
            b.Append(new string(' ', 9)); //09-17
            b.AppendNumero(6, ++qtdeLinhasLote); //18-23

            b.AppendDinheiro(16, liquidacao.ValorPagamento); //24-41        O QUE INFORMAR AQUI?
            b.AppendNumero(13, 0); //42-59                                  O QUE INFORMAR AQUI?
            b.AppendNumero(6, 0); //60-65                                   O QUE INFORMAR AQUI?

            b.Append(new string(' ', 175)); //66-240
            b.Append(Environment.NewLine);
        }

        //public void HeaderBoleto(StringBuilder b, Liquidacao liquidacao)
        //{
        //    ++qtdeLinhasArquivo; //NAO APAGAR
        //    ++qtdeLinhasLote; //NAO APAGAR

        //    b.Append("237"); //01-03
        //    b.AppendNumero(4, ++codigoLote); //04-07
        //    b.Append('1'); //08-08
        //    b.Append('C'); //09-09 (C=Credito)
        //    b.AppendNumero(2, 20); //10-11 TIPO DE PAGTO
        //    b.AppendNumero(2, liquidacao.FormaPagamento); //12-13 FORMA DE PAGAMENTO
        //    b.Append("030"); //14-16
        //    b.Append(' '); //17-17
        //    b.Append('2'); //18-18
        //    b.AppendNumero(14, Opcoes.CnpjPagador); //19-32
        //    b.Append(new string(' ', 20)); //033-052
        //    b.AppendNumero(5, Opcoes.NumeroAgencia); //53-57
        //    b.Append(' '); //58-58
        //    b.AppendNumero(12, Opcoes.NumeroContaCorrente); //59-70
        //    b.Append(' '); //71-71
        //    b.AppendNumero(1, Opcoes.DAC);//72-72
        //    b.AppendTexto(30, Opcoes.RazaoSocial); //73-102
        //    b.Append(new string(' ', 30)); //103-132 FINALIDADE DO LOTE
        //    b.Append(new string(' ', 10)); //133-142 HISTÓRICO DE C/C
        //    b.AppendTexto(30, Opcoes.EnderecoPagador); //143-172
        //    b.AppendNumero(5, 0); //173-177 NUMERO
        //    b.Append(new string(' ', 15));  //178-192
        //    b.AppendTexto(20, Opcoes.Cidade); //193-212
        //    b.AppendNumero(8, Opcoes.Cep); //213-220
        //    b.AppendTexto(2, Opcoes.UF); //221-222
        //    b.Append(new string(' ', 8)); //223-230
        //    b.Append(new string(' ', 10)); //231-240
        //    b.Append(Environment.NewLine);
        //}

        //public void DetalheBoleto(StringBuilder b, Liquidacao liquidacao) // Segmento J
        //{
        //    ++qtdeLinhasArquivo; //NAO APAGAR
        //    ++qtdeLinhasLote; //NAO APAGAR

        //    b.Append("237"); //01-03
        //    b.AppendNumero(4, codigoLote); //04-07
        //    b.Append('3');
        //    b.AppendNumero(5, ++codigoDetalhe);
        //    b.Append('J');
        //    b.Append("000"); //TIPO DE MOVIMENTO (000 = Inclusão de pagamento)
        //    b.AppendTexto(44, FormataCodigoBarras(liquidacao));
        //    b.AppendTexto(30, liquidacao.NomeFavorecido);
        //    b.AppendData(liquidacao.DataVencimento);
        //    b.AppendDinheiro(15, liquidacao.ValorPagamento);
        //    b.AppendDinheiro(15, liquidacao.ValorDesconto);
        //    b.AppendTexto(15, CalcularMultaMora(liquidacao));
        //    b.AppendData(liquidacao.DataPagamento);
        //    b.AppendDinheiro(15, liquidacao.ValorPagamento);
        //    b.AppendNumero(15, 0);
        //    b.AppendTexto(20, liquidacao.Documento); // Seu Numero
        //    b.Append(new string(' ', 13));
        //    b.Append(new string(' ', 15));
        //    b.Append(new string(' ', 10));
        //    b.Append(Environment.NewLine);
        //}

        //public void DetalheBoletoOnline(StringBuilder b, Liquidacao liquidacao) // Segmento J-52
        //{
        //    ++qtdeLinhasArquivo; //NAO APAGAR
        //    ++qtdeLinhasLote; //NAO APAGAR

        //    b.Append("237"); //01-03
        //    b.AppendNumero(4, codigoLote); //04-07
        //    b.Append('3');
        //    b.AppendNumero(5, ++codigoDetalhe);
        //    b.Append('J');
        //    b.Append("000"); //TIPO DE MOVIMENTO (000 = Inclusão de pagamento)
        //    b.Append("52"); // Identificação do Registro Opcional
        //    b.Append('2');
        //    b.Append('0');
        //    b.AppendNumero(14, Opcoes.CnpjPagador);
        //    b.AppendTexto(40, Opcoes.RazaoSocial);
        //    b.AppendTexto(1, ChecaInscricaoEmp(liquidacao.CpfCnpjFavorecido)); // Beneficiário
        //    b.AppendTexto(15, CnpjOuCpf(liquidacao.CpfCnpjFavorecido));
        //    b.AppendTexto(40, liquidacao.NomeFavorecido);
        //    b.AppendTexto(1, ChecaInscricaoEmp(liquidacao.CpfCnpjFavorecido)); // Sacador Avalista
        //    b.AppendTexto(15, CnpjOuCpf(liquidacao.CpfCnpjFavorecido));
        //    b.AppendTexto(40, liquidacao.NomeFavorecido);
        //    b.Append(new string(' ', 53));
        //    b.Append(Environment.NewLine);
        //}

        //public void TrailerBoleto(StringBuilder b, Liquidacao liquidacao)
        //{
        //    ++qtdeLinhasArquivo; //NAO APAGAR

        //    b.Append("237");
        //    b.AppendNumero(4, codigoLote);
        //    b.Append('5');
        //    b.Append(new string(' ', 9));
        //    b.AppendNumero(6, ++qtdeLinhasLote);
        //    b.AppendDinheiro(18, liquidacao.ValorPagamento);
        //    b.AppendNumero(18, 0);
        //    b.Append(new string(' ', 171));
        //    b.Append(new string(' ', 10));
        //    b.Append(Environment.NewLine);
        //}

        public void TrailerArquivo(StringBuilder b)
        {
            b.Append("237"); //01-03
            b.Append("9999"); //04-07
            b.Append('9'); //08-08
            b.Append(new string(' ', 9)); //09-17
            b.AppendNumero(6, codigoLote); //18-23

            b.AppendNumero(6, ++qtdeLinhasArquivo); //24-29     O QUE INFORMAR AQUI? NAO CONTA A LIQUIDACAO?

            b.AppendNumero(6, 0); //30-35                       O QUE INFORMAR AQUI?

            b.Append(new string(' ', 205)); //36-240
            b.Append(Environment.NewLine);                      //QUAL O LAYOUT PARA BOLETO?
        }

        //private string FazerAgenciaContaFavorecido(Liquidacao liquidacao)
        //{
        //    var texto = new StringBuilder();

        //    if (liquidacao.BancoFavorecido == "341" || liquidacao.BancoFavorecido == "409")
        //    {
        //        texto.AppendNumero(1, 0);
        //        texto.AppendNumero(4, liquidacao.AgenciaFavorecido);
        //        texto.Append(' ');
        //        texto.AppendNumero(6, 0);

        //        if (liquidacao.FormaPagamento == "02" || liquidacao.FormaPagamento == "10")
        //        {
        //            texto.AppendNumero(6, 0);
        //            texto.Append(' ');
        //            texto.AppendNumero(1, 0);
        //        }
        //        else
        //        {
        //            texto.AppendNumero(6, liquidacao.ContaFavorecido);
        //            texto.Append(' ');
        //            texto.AppendNumero(1, liquidacao.DacFavorecido);
        //        }
        //    }
        //    else
        //    {
        //        texto.AppendNumero(5, liquidacao.AgenciaFavorecido);
        //        texto.Append(' ');
        //        texto.AppendNumero(12, liquidacao.ContaFavorecido);
        //        if (liquidacao.DacFavorecido.Length > 1)
        //        {
        //            texto.AppendNumero(2, liquidacao.DacFavorecido);
        //        }
        //        else
        //        {
        //            texto.Append(' ');
        //            texto.AppendNumero(1, liquidacao.DacFavorecido);
        //        }
        //    }

        //    return (texto.ToString());
        //}

        private string RetornaDigitoCC(int posicao, string digito)
        {
            string d = "";

            if (posicao == 1)
            {
                if (digito.Length == 2)
                    d = digito.Substring(0, 1);
                else
                    d = digito;
            }
            else
            {
                if (digito.Length == 2)
                    d = digito.Substring(1, 1);
                else
                    d = digito;
            }

            return d;
        }

        public string ValidaPagto(string formaPagto, string numeroBanco, bool corretora, string codBarras)
        {
            var x = "";

            if (formaPagto == "01" && numeroBanco != "237")
            {
                x = "Para pagto via Crédito em Conta, é necessário que todos os Fornecedores selecionados tenham conta no banco Bradesco!";
            }
            //if (formaPagto == "41B" && corretora == false)
            //{
            //    x = "Para pagto via TED P/ Corretora, é necessário que todos os Fornecedores selecionados tenham conta corretora";
            //}
            if ((formaPagto == "30" || formaPagto == "31") && codBarras == "")
            {
                x = "Para pagto via Boleto, é necessário que todos os Títulos selecionados tenham Código de Barras informado.";
            }

            return x;
        }

        private string ChecaInscricaoEmp(string cnpjOuCpf)
        {
            string inscricaoEmpresa = cnpjOuCpf.Length == 11 ? "1" : "2";

            return inscricaoEmpresa;
        }

        //private string CincoPrimeirosCnpj(string cnpj)
        //{
        //    string numeros = cnpj.Substring(0, 5);

        //    return numeros;
        //}

        //private string CnpjOuCpf(string cnpjOuCpf)
        //{
        //    var texto = new StringBuilder();
        //    var inscricaoEmpresa = ChecaInscricaoEmp(cnpjOuCpf);

        //    if (inscricaoEmpresa == "1")
        //    {
        //        texto.AppendTexto(11, cnpjOuCpf);
        //        texto.Append(new string(' ', 4));
        //    }
        //    else
        //    {
        //        texto.Append('0');
        //        texto.AppendTexto(14, cnpjOuCpf);
        //    }

        //    return (texto.ToString());
        //}

        //private string CalcularMultaMora(Liquidacao liquidacao)
        //{
        //    var texto = new StringBuilder();

        //    var valorMultaMora = liquidacao.Multa + liquidacao.Mora;
        //    texto.AppendDinheiro(15, valorMultaMora);

        //    return (texto.ToString());
        //}

        private bool ValidaDvGeral(string codBarras)
        {
            string parteUm = codBarras.Substring(0, 4);
            int dv = Convert.ToInt32(codBarras.Substring(4, 1));
            string parteDois = codBarras.Substring(5, 39);

            string codigo = parteUm + parteDois;

            bool retorno;
            int mult = 0, total = 0, posicao = 1, limite = 9, digito = 0, resto = 0;
            string num = string.Empty;

            mult += 1 + (codigo.Length % (limite - 1));

            if (mult == 1)
                mult = limite;

            while (posicao <= codigo.Length)
            {
                num = Mid(codigo, posicao, 1);
                total += Convert.ToInt32(num) * mult;

                mult -= 1;
                if (mult == 1)
                    mult = limite;

                posicao += 1;
            }

            resto += (total % 11);

            if (resto == 0 || resto == 1 || resto == 10 || resto == 11)
                digito += 1;
            else
                digito += (11 - resto);

            if (dv == digito)
                retorno = true;
            else
                retorno = false;

            return retorno;
        }

        private bool ValidaDvUnitario(string codBarras)
        {
            bool retorno;

            string campoUm = codBarras.Substring(0, 9);
            int dvUm = Convert.ToInt32(codBarras.Substring(9, 1));
            string campoDois = codBarras.Substring(10, 10);
            int dvDois = Convert.ToInt32(codBarras.Substring(20, 1));
            string campoTres = codBarras.Substring(21, 10);
            int dvTres = Convert.ToInt32(codBarras.Substring(31, 1));

            int somaUm = 0, somaDois = 0, somaTres = 0, pesoUm = 2, pesoDois = 2, pesoTres = 2, restoUm, restoDois, restoTres;

            for (int i = campoTres.Length; i > 0; i--)
            {
                restoTres = (Convert.ToInt32(Mid(campoTres, i, 1)) * pesoTres);

                if (restoTres > 9)
                    restoTres = (restoTres / 10) + (restoTres % 10);

                somaTres += restoTres;

                if (pesoTres == 2)
                    pesoTres = 1;
                else
                    pesoTres = pesoTres + 1;
            }
            int dvCampo3 = ((10 - (somaTres % 10)) % 10);

            for (int i = campoDois.Length; i > 0; i--)
            {
                restoDois = (Convert.ToInt32(Mid(campoDois, i, 1)) * pesoDois);

                if (restoDois > 9)
                    restoDois = (restoDois / 10) + (restoDois % 10);

                somaDois += restoDois;

                if (pesoDois == 2)
                    pesoDois = 1;
                else
                    pesoDois = pesoDois + 1;
            }
            int dvCampo2 = ((10 - (somaDois % 10)) % 10);

            for (int i = campoUm.Length; i > 0; i--)
            {
                restoUm = (Convert.ToInt32(Mid(campoUm, i, 1)) * pesoUm);

                if (restoUm > 9)
                    restoUm = (restoUm / 10) + (restoUm % 10);

                somaUm += restoUm;

                if (pesoUm == 2)
                    pesoUm = 1;
                else
                    pesoUm = pesoUm + 1;
            }
            int dvCampo1 = ((10 - (somaUm % 10)) % 10);

            if (dvCampo3 == 10)
                dvCampo3 = 0;
            if (dvCampo2 == 10)
                dvCampo2 = 0;
            if (dvCampo1 == 10)
                dvCampo1 = 0;

            if (dvTres == dvCampo3 && dvDois == dvCampo2 && dvUm == dvCampo1)
                retorno = true;
            else
                retorno = false;

            return retorno;
        }

        public static string Mid(string str, int start, int? length = null)
        {
            if (!length.HasValue)
                return str.Substring(start - 1);
            else
                return str.Substring(start - 1, length.Value);
        }

        public bool ValidaCodBarras(string codBarras)
        {
            bool valido;

            if (codBarras.Length == 44)
                valido = ValidaDvGeral(codBarras);
            else if (codBarras.Length == 47)
            {
                bool dvGeral, dvUnitario;

                dvUnitario = ValidaDvUnitario(codBarras);
                string codBarrasTxt = RetornaCodigoFormatado(codBarras);
                dvGeral = ValidaDvGeral(codBarrasTxt);

                if (dvUnitario == true && dvGeral == true)
                    valido = true;
                else
                    valido = false;
            }
            else
            {
                valido = false;
            }

            return valido;
        }

        private string RetornaCodigoFormatado(string codBarras)
        {
            string codBanco = codBarras.Substring(0, 3);
            string codMoeda = codBarras.Substring(3, 1);

            string campo1 = codBarras.Substring(4, 5);
            string campo2 = codBarras.Substring(10, 10);
            string campo3 = codBarras.Substring(21, 10);
            string campoLivre = campo1 + campo2 + campo3;

            string dv = codBarras.Substring(32, 1);
            string fatVencto = codBarras.Substring(33, 4);
            string valor = codBarras.Substring(37, 10);

            string texto = codBanco + codMoeda + dv + fatVencto + valor + campoLivre;

            return texto;
        }

        //private string FormataCodigoBarras(Liquidacao liquidacao)
        //{
        //    var texto = new StringBuilder();

        //    string codBanco, codMoeda, dv, fatVencto, valor, campoLivre;

        //    if (liquidacao.CodigoBarras.Length == 44)
        //    {
        //        codBanco = liquidacao.CodigoBarras.Substring(0, 3);
        //        codMoeda = liquidacao.CodigoBarras.Substring(3, 1);
        //        dv = liquidacao.CodigoBarras.Substring(4, 1);
        //        fatVencto = liquidacao.CodigoBarras.Substring(5, 4);
        //        valor = liquidacao.CodigoBarras.Substring(6, 10);
        //        campoLivre = liquidacao.CodigoBarras.Substring(7, 25);
        //    }
        //    else
        //    {
        //        codBanco = liquidacao.CodigoBarras.Substring(0, 3);
        //        codMoeda = liquidacao.CodigoBarras.Substring(3, 1);

        //        string campo1 = liquidacao.CodigoBarras.Substring(4, 5);
        //        string campo2 = liquidacao.CodigoBarras.Substring(10, 10);
        //        string campo3 = liquidacao.CodigoBarras.Substring(21, 10);
        //        campoLivre = campo1 + campo2 + campo3;

        //        dv = liquidacao.CodigoBarras.Substring(32, 1);
        //        fatVencto = liquidacao.CodigoBarras.Substring(33, 4);
        //        valor = liquidacao.CodigoBarras.Substring(37, 10);
        //    }

        //    texto.AppendTexto(3, codBanco);
        //    texto.AppendTexto(1, codMoeda);
        //    texto.AppendTexto(1, dv);
        //    texto.AppendTexto(4, fatVencto);
        //    texto.AppendTexto(10, valor);
        //    texto.AppendTexto(25, campoLivre);

        //    return (texto.ToString());
        //}        
    }
}