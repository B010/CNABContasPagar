using CnabContasPagar.Bancos;
using CnabContasPagar.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasPagar.Testes.Itau
{
    public class HeaderTrailerBoletoTests
    {
        private string _linha1;
        private string _linha2;
        private string _linha3;
        private string _linha4;

        public HeaderTrailerBoletoTests()
        {
            _linha1 = GerarLinhaHeader(Liquidacao1());
            _linha2 = GerarLinhaHeader(Liquidacao2());

            _linha3 = GerarLinhaTrailer(Liquidacao1());
            _linha4 = GerarLinhaTrailer(Liquidacao2());
        }

        [Fact]
        public void HeaderTem400Caracteres()
        {
            Assert.Equal(240, _linha1.Length - 2); //o enter pra linha de baixo conta como 2
            Assert.Equal(240, _linha2.Length - 2);
        }
        [Fact]
        public void TrailerTem400Caracteres()
        {
            Assert.Equal(240, _linha3.Length - 2); //o enter pra linha de baixo conta como 2
            Assert.Equal(240, _linha4.Length - 2);
        }

        public static string GerarLinhaHeader(Liquidacao liquidacao)
        {
            var cnab = new BancoItau240(Opcoes());
            var sb = new StringBuilder();
            cnab.HeaderBoleto(sb, liquidacao);

            return sb.ToString();
        }

        public static string GerarLinhaTrailer(Liquidacao liquidacao)
        {
            var cnab = new BancoItau240(Opcoes());
            var sb = new StringBuilder();
            cnab.TrailerBoleto(sb, liquidacao);

            return sb.ToString();
        }

        public static Opcoes Opcoes()
        {
            return new Opcoes
            {
                NumeroAgencia = "8380",
                NumeroContaCorrente = "1558",
                RazaoSocial = "EMPRESA TAL LTDA",
                Bairro = "Bairro",
                Cep = "12300000",
                Cidade = "Cidade",
                DAC = "2",
                NomeBanco = "ITAU",
                Numero = "00000",
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
                CpfCnpjFavorecido = "32140856000159",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "41",
                ValorPagamento = 120m,
                InscricaoEmpresa = "2",
                Documento = "66662",
                CodigoBarras = "34196166700000123451101234567880057123457000",
                ValorDesconto = 25m,
                Multa = 0m,
                Mora = 0m,
                DataVencimento = new DateTime(2022, 11, 10)
            };
        }

        public static Liquidacao Liquidacao2()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "2332",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32140856000159",
                DacFavorecido = "1",
                DataPagamento = new DateTime(2020, 11, 10),
                NomeFavorecido = "LOJAS 2 LTDA",
                NossoNumero = "344",
                FormaPagamento = "41",
                ValorPagamento = 11m,
                InscricaoEmpresa = "2",
                Documento = "66662",
                CodigoBarras = "34196166700000123451101234567880057123457000",
                ValorDesconto = 25m,
                Multa = 0m,
                Mora = 0m,
                DataVencimento = new DateTime(2022, 11, 10)
            };
        }

    }
}
