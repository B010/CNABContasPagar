using CnabContasPagar.Models;
using CnabContasPagar.Util;
using CNABContasPagar.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Bancos
{
    public class BancoBradesco500 : IBanco
    {
        private int qtdeLinhasArquivo = 1;

        public BancoBradesco500(Opcoes opcoes)
        {
            Opcoes = opcoes;
        }

        public Opcoes Opcoes { get; set; }

        public string MontarArquivo(List<Liquidacao> liquidacoes)
        {
            qtdeLinhasArquivo = 0;

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
            b.Append("0"); //01-01

            b.AppendNumero(8, 0); //02-09 Codigo de Comunicacao                     -00127210 - Qual o Codigo?

            b.Append('2'); //10-10 (1-CPF, 2-CNPJ)
            b.AppendNumero(15, Opcoes.CnpjPagador); //11-25
            b.AppendTexto(40, Opcoes.RazaoSocial.ToUpper()); //26-65
            b.Append("20"); //66-67 Pagto a Fornecedores
            b.Append("1"); //68-68

            b.AppendNumero(5, "0"); //69-73 Numero da Remessa                       -00794 -Como funciona essa sequencia? 

            b.Append("00000"); //74-78
            b.AppendData(DateTime.Now, "yyyyMMdd"); //79-86
            b.AppendData(DateTime.Now, "hhmmss"); //87-92
            b.Append(new string(' ', 5)); //93-97
            b.Append(new string(' ', 3)); //98-100
            b.Append(new string(' ', 5)); //101-105
            b.Append("0"); //106-106

            b.Append(new string(' ', 74)); //107-180 Reservado para Empresa         -Posso deixar em branco?

            b.Append(new string(' ', 80)); //181-260
            b.Append(new string(' ', 217)); //261-477

            b.AppendNumero(9, "0"); //478-486 Numero da Lista de Debito             -000000794 -Como funciona essa sequencia? 

            b.Append(new string(' ', 8)); //487-494
            b.AppendNumero(6, ++qtdeLinhasArquivo); //495-500
            b.Append(Environment.NewLine);
        }

        public void DetalhePagamentoComum(StringBuilder b, Liquidacao liquidacao)
        {
            DetalheA(b, liquidacao);
        }

        public void HeaderDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {

        }

        public void DetalheA(StringBuilder b, Liquidacao liquidacao)
        {
            b.Append("1"); //01-01
            b.AppendTexto(1, ChecaInscricaoEmp(liquidacao.CpfCnpjFavorecido)); //02-02
            b.AppendNumero(15, liquidacao.CpfCnpjFavorecido); //03-17
            b.AppendTexto(30, liquidacao.NomeFavorecido.ToUpper()); //18-47
            b.AppendTexto(40, liquidacao.EnderecoFavorecido.ToUpper()); //48-87
            b.AppendNumero(8, liquidacao.CepFavorecido); //88-95
            b.AppendNumero(3, liquidacao.BancoFavorecido); //96-98
            b.AppendNumero(5, liquidacao.AgenciaFavorecido); //99-103
            b.AppendNumero(1, liquidacao.DigitoAgenciaFavorecido); //104-104
            b.AppendNumero(13, liquidacao.ContaFavorecido); //105-117
            b.AppendNumero(1, liquidacao.DacFavorecido); //118-118
            b.Append(new string(' ', 1)); //119-119

            b.Append(new string(' ', 16)); //120-135 Numero do Pagamento                           O que colocar aqui?

            if (liquidacao.FormaPagamento == "31")
            {
                b.AppendNumero(3, Carteira(liquidacao.CodigoBarras)); //136-138 Carteira
            }
            if (liquidacao.FormaPagamento == "30")
            {
                b.Append("000"); //136-138 Carteira                                                "Consta do Arquivo de Rastreamento"
                                                                                                   //O que colocar aqui? Onde consigo esse arquivo?
            }
            else
            {
                b.Append("000"); //136-138 Carteira
            }

            if (liquidacao.FormaPagamento == "31")
            {
                b.AppendNumero(12, NossoNumero(liquidacao.CodigoBarras)); //139-150 Nosso Numero
            }
            else
            {
                b.AppendNumero(12, "0"); //139-150 Nosso Numero
            }

            if (liquidacao.FormaPagamento == "30")
            {
                b.Append(new string(' ', 15)); //151-165 Seu Numero                                O que colocar aqui?
            }
            else
            {
                b.Append(new string(' ', 15)); //151-165 Seu Numero
            }

            b.AppendData(liquidacao.DataVencimento, "yyyyMMdd"); //166-173
            b.AppendNumero(8, "0"); //174-181
            b.AppendData(liquidacao.DataVencimento, "yyyyMMdd"); //182-189
            b.Append('0'); //190-190
            b.Append(FatorVencimento(liquidacao.CodigoBarras)); //191-194

            if (liquidacao.FormaPagamento == "31")
            {
                b.AppendNumero(10, ValorConstante(liquidacao.CodigoBarras)); //195-204 Valor Documento
            }
            if (liquidacao.FormaPagamento == "30")
            {
                b.AppendNumero(10, "0"); //195-204 Valor Documento                                  O que colocar aqui?
            }
            else
            {
                b.AppendDinheiro(10, CalculaValores(liquidacao.ValorPagamento, liquidacao.Multa, liquidacao.Mora, "DOC")); //195-204 Valor Documento
            }

            b.AppendDinheiro(15, CalculaValores(liquidacao.ValorPagamento, liquidacao.Multa, liquidacao.Mora, "DOC")); //205-219 Valor Pagto
            b.AppendDinheiro(15, 0); //220-234 Valor Desconto
            b.AppendDinheiro(15, CalculaValores(liquidacao.ValorPagamento, liquidacao.Multa, liquidacao.Mora, "VA")); //195-204 Valor Acrescimo
            b.Append("01"); //250-251 Tipo Doc
            b.AppendTexto(12, liquidacao.Documento); //252-263 Numero Nota
            b.AppendNumero(2, liquidacao.FormaPagamento); //264-265
            b.AppendData(liquidacao.DataPagamento, "yyyyMMdd"); //266-273
            b.Append(new string(' ', 3)); //274-276
            b.Append("01"); //277-278
            b.Append(new string(' ', 10)); //279-288
            if (liquidacao.FormaPagamento == "30")
            {
                b.Append("5"); //289-289
            }
            else
            {
                b.Append("1"); //289-289
            }
            b.Append("00"); //290-291
            b.Append(new string(' ', 4)); //292-295
            b.Append(new string(' ', 15)); //296-310
            b.Append(new string(' ', 15)); //311-325
            b.Append(new string(' ', 6)); //326-331
            b.AppendTexto(40, liquidacao.NomeFavorecido.ToUpper()); //332-371
            b.Append(new string(' ', 1)); //372-372
            b.Append(new string(' ', 1)); //373-373
            if (liquidacao.FormaPagamento == "01")
            {
                b.Append(new string(' ', 40)); //374-413
            }
            if (liquidacao.FormaPagamento == "03" || liquidacao.FormaPagamento == "08")
            {
                b.Append("C"); //374-374
                b.Append("000000"); //375-380
                b.Append("07"); //381-382 Pagto Fornecedor
                b.Append("01"); //383-384 Conta Corrente Individual
                b.AppendNumero(18, 0); //385-402
                b.Append(new string(' ', 11)); //403-413
            }
            if (liquidacao.FormaPagamento == "30")
            {
                b.Append(new string(' ', 25)); //374-398
                b.AppendNumero(15, liquidacao.CpfCnpjFavorecido); //399-413
            }
            else
            {
                b.AppendNumero(25, CampoLivre(liquidacao.CodigoBarras)); //374-398                          Duvida na funcao!!
                b.AppendNumero(1, DigitoVerificador(liquidacao.CodigoBarras)); //399-399
                b.AppendNumero(1, CodigoMoeda(liquidacao.CodigoBarras)); //400-400
                b.Append(new string(' ', 13)); //401-413
            }
            b.AppendNumero(2, "0"); //414-415
            b.Append(new string(' ', 35)); //416-450
            b.Append(new string(' ', 22)); //451-472

            b.AppendNumero(5, "0"); //473-477                                                   O que colocar aqui? Quais os codigos já cadastrados?

            b.Append(new string(' ', 1)); //478-478
            if (liquidacao.FormaPagamento == "01")
            {
                b.Append("1"); //479-479
            }
            else
            {
                b.Append("0"); //479-479
            }

            b.Append("0000000"); //480-486 Conta Complementar                                  Qual o codigo?

            b.Append(new string(' ', 8)); //487-494
            b.AppendNumero(6, ++qtdeLinhasArquivo); //495-500
            b.Append(Environment.NewLine);
        }

        public void DetalheB(StringBuilder b, Liquidacao liquidacao)
        {

        }

        public void TrailerDetalheComum(StringBuilder b, Liquidacao liquidacao)
        {

        }

        public void TrailerArquivo(StringBuilder b)
        {
            decimal valorSomado = 0;

            b.Append("9"); //01-01
            b.AppendNumero(6, ++qtdeLinhasArquivo); //02-07 Qtde Registros
            b.AppendDinheiro(17, valorSomado); //08-24                              Soma de todos os valores a serem pagos nos Registros de Transacao?
            b.Append(new string(' ', 470)); //25-494
            b.AppendNumero(6, qtdeLinhasArquivo); //495-500 Numero Sequencial
            b.Append(Environment.NewLine);
        }

        private string CodigoMoeda(string codBarras)
        {
            //291 9 7 1044 0000200000 0417090001260000600957300

            string d = "0";

            if (codBarras != "")
                d = codBarras.Substring(3, 1);

            return d;
        }

        private string Carteira(string codBarras)
        {
            string d = "0", banco = "";

            if (codBarras != "")
            {
                banco = codBarras.Substring(0, 3);

                if (banco == "237")
                {
                    if (codBarras.Length == 44)
                    {
                        d = codBarras.Substring(23, 2);
                    }
                    if (codBarras.Length == 47)
                    {
                        var um = codBarras.Substring(8, 1);
                        var dois = codBarras.Substring(10, 1);

                        d = um + dois;
                    }
                }
            }

            return d;
        }

        private string NossoNumero(string codBarras)
        {
            string d = "0", banco = "";

            if (codBarras != "")
            {
                banco = codBarras.Substring(0, 3);

                if (banco == "237")
                {
                    if (codBarras.Length == 44)
                    {
                        d = codBarras.Substring(25, 11);
                    }
                    if (codBarras.Length == 47)
                    {
                        var um = codBarras.Substring(11, 9);
                        var dois = codBarras.Substring(21, 2);

                        d = um + dois;
                    }
                }
            }

            return d;
        }

        private string DigitoVerificador(string codBarras)
        {
            string d = "0";

            if (codBarras != "")
            {
                if (codBarras.Length == 44)
                {
                    d = codBarras.Substring(4, 1);
                }
                if (codBarras.Length == 47)
                {
                    d = codBarras.Substring(32, 1);
                }
            }

            return d;
        }

        private string ValorConstante(string codBarras)
        {
            string d = "0";

            if (codBarras != "")
            {
                if (codBarras.Length == 44)
                {
                    d = codBarras.Substring(9, 10);
                }
                if (codBarras.Length == 47)
                {
                    d = codBarras.Substring(37, 10);
                }
            }

            return d;
        }

        private string FatorVencimento(string codBarras)
        {
            string d = "0000";

            if (codBarras != "")
            {
                if (codBarras.Length == 44)
                {
                    d = codBarras.Substring(5, 4);
                }
                if (codBarras.Length == 47)
                {
                    d = codBarras.Substring(33, 4);
                }
            }

            return d;
        }

        private string CampoLivre(string codBarras)
        {
            string d = "0", banco = "";

            if (codBarras != "")
            {
                if (codBarras.Length == 44)
                {
                    d = codBarras.Substring(19, 25);
                }
                if (codBarras.Length == 47)
                {
                    banco = codBarras.Substring(0, 3);

                    if (banco == "237")
                    {
                        //Exemplo da Linha digitável padrão Bradesco, pag 46

                        //No exemplo, o Campo Livre tem 31 posicoes (sem contar os 3 digitos)

                        //Qual é o Campo Livre?
                    }
                    else
                    {
                        var um = codBarras.Substring(4, 5);
                        var dois = codBarras.Substring(10, 10);
                        var tres = codBarras.Substring(21, 10);

                        d = um + dois + tres;
                    }
                }
            }

            return d;
        }

        private decimal CalculaValores(decimal valor, decimal multa, decimal mora, string calculo)
        {
            decimal d = 0;

            if (calculo == "DOC")
            {
                if (multa != 0 || mora != 0)
                    d = valor + multa + mora;
                else
                    d = valor;
            }

            if (calculo == "VA")
            {
                if (multa != 0 || mora != 0)
                    d = multa + mora;
                else
                    d = 0;
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
            if (formaPagto == "31" && codBarras == "")
            {
                x = "Para pagto de Boletos de Outros Bancos, é necessário que todos os Títulos selecionados tenham Código de Barras informado.";
            }

            return x;
        }

        private string ChecaInscricaoEmp(string cnpjOuCpf)
        {
            string inscricaoEmpresa = cnpjOuCpf.Length == 11 ? "1" : "2";

            return inscricaoEmpresa;
        }

        private bool ValidaDvGeral(string codBarras)
        {
            string banco = codBarras.Substring(0, 3);

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

            if (banco == "237")
            {
                var resultado = 11 - resto;

                if (resultado == 0 || resultado == 1 || resultado > 9)
                    digito = 1;
                else
                    digito = resultado;
            }
            else
            {
                if (resto == 0 || resto == 1 || resto > 9)
                    digito += 1;
                else
                    digito += (11 - resto);
            }

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
    }
}