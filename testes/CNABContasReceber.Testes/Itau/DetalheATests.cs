using CnabContasPagar.Bancos;
using CnabContasPagar.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasPagar.Testes.Itau
{
    public class DetalheATests
    {
        private string _linha1;
        private string _linha2;
        private string _linha3;
        private string _linha4;

        public DetalheATests()
        {
            _linha1 = GerarLinhaDetalhe(Liquidacao1());
            _linha2 = GerarLinhaDetalhe(Liquidacao2());
            _linha3 = GerarLinhaDetalhe(Liquidacao3());
            _linha4 = GerarLinhaDetalhe(Liquidacao4());
        }

        [Fact]
        public void Tem400Caracteres()
        {
            Assert.Equal(240, _linha1.Length - 2); //o enter pra linha de baixo conta como 2
            Assert.Equal(240, _linha2.Length - 2);
            Assert.Equal(240, _linha3.Length - 2);
            Assert.Equal(240, _linha4.Length - 2);
        }

        [Fact]
        public void Escreveu_Valor_Correto()
        {
            var linha = GerarLinhaDetalhe(Liquidacao1());
            var valor = linha.Slice(120, 134);

            Assert.Equal("000000000012000", valor);

        }

        [Fact]
        public void Escreveu_Data_Correta()
        {
            var linha = GerarLinhaDetalhe(Liquidacao1());
            var valor = linha.Slice(94, 101);

            Assert.Equal("10102020", valor);

        }

        public static string GerarLinhaDetalhe(Liquidacao liquidacao)
        {
            var cnab = new BancoItau240(Opcoes());
            var sb = new StringBuilder();
            cnab.DetalheA(sb, liquidacao);

            return sb.ToString();
        }

        public static Opcoes Opcoes()
        {
            return new Opcoes
            {
                NumeroAgencia = "8380",
                NumeroContaCorrente = "1558",
                RazaoSocial = "EMPRESA TAL LTDA",
                Bairro = "1",
                Cep = "123",
                Cidade = "Cidade",
                DAC = '2',
                NomeBanco = "ITAU",
                Numero = "1",
                UF = "SP",
                EnderecoPagador = "LOGO ALI",
                CnpjPagador = "1231313"
            };
        }

        public static Liquidacao Liquidacao1()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "1233",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32.140.856/0001-59",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "02",
                ValorPagamento = 120m
            };
        }

        public static Liquidacao Liquidacao2()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "2332",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32.140.856/0001-59",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "02",
                ValorPagamento = 10m
            };
        }

        public static Liquidacao Liquidacao3()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "1233",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32.140.856/0001-59",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "02",
                ValorPagamento = 10m
            };
        }

        public static Liquidacao Liquidacao4()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "0233",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32.140.856/0001-59",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "02",
                ValorPagamento = 10m
            };
        }

    }
}
