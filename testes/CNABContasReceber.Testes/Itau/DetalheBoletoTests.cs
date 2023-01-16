using CnabContasPagar.Bancos;
using CnabContasPagar.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasPagar.Testes.Itau
{
    public class DetalheBoletoTests
    {
        private string _linha1;
        private string _linha2;
        private string _linha3;
        private string _linha4;

        public DetalheBoletoTests()
        {
            _linha1 = GerarLinhaDetalhe(Liquidacao1());
            _linha2 = GerarLinhaDetalhe(Liquidacao2());
            _linha3 = GerarLinhaDetalheBoleto(Liquidacao3());
            _linha4 = GerarLinhaDetalheBoleto(Liquidacao4());
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
            var valor = linha.Slice(100, 114);

            Assert.Equal("000000000012000", valor);

        }

        [Fact]
        public void Escreveu_Data_Correta()
        {
            var linha = GerarLinhaDetalhe(Liquidacao1());
            var valor = linha.Slice(92, 99);

            Assert.Equal("10102022", valor);

        }

        public static string GerarLinhaDetalhe(Liquidacao liquidacao)
        {
            var cnab = new BancoItau240(Opcoes());
            var sb = new StringBuilder();
            cnab.DetalheBoleto(sb, liquidacao); // segmento J

            return sb.ToString();
        }
        
        public static string GerarLinhaDetalheBoleto(Liquidacao liquidacao)
        {
            var cnab = new BancoItau240(Opcoes());
            var sb = new StringBuilder();
            cnab.DetalheBoleto(sb, liquidacao); // segmento J-52

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
                ContaFavorecido = "123763",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32140856000159",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "30",
                ValorPagamento = 120m,
                InscricaoEmpresa = "2",
                Documento = "66662",
                CodigoBarras = "34196166700000123451101234567880057123457000",
                ValorDesconto = 25m,
                Multa = 0m,
                Mora = 0m,
                DataVencimento = new DateTime(2022, 10, 10)
            };
        }

        public static Liquidacao Liquidacao2()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "233278",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32140856000159",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "31",
                ValorPagamento = 10m,
                InscricaoEmpresa = "2",
                Documento = "666666",
                CodigoBarras = "34196166700000123451101234567880057123457000",
                ValorDesconto = 25m,
                Multa = 0m,
                Mora = 0m,
                DataVencimento = new DateTime(2022, 10, 10)
            };
        }

        public static Liquidacao Liquidacao3()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "128733",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32140856000159",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "30",
                ValorPagamento = 10m,
                InscricaoEmpresa = "2",
                Documento = "66662",
                CodigoBarras = "34196166700000123451101234567880057123457000",
                ValorDesconto = 25m,
                Multa = 0m,
                Mora = 0m,
                DataVencimento = new DateTime(2022, 10, 10)
            };
        }

        public static Liquidacao Liquidacao4()
        {
            return new Liquidacao()
            {
                AgenciaFavorecido = "1233",
                ContaFavorecido = "023098",
                BancoFavorecido = "341",
                CpfCnpjFavorecido = "32140856000159",
                DacFavorecido = "3",
                DataPagamento = new DateTime(2020, 10, 10),
                NomeFavorecido = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                FormaPagamento = "31",
                ValorPagamento = 10m,
                InscricaoEmpresa = "2",
                Documento = "66662",
                CodigoBarras = "34196166700000123451101234567880057123457000",
                ValorDesconto = 25m,
                Multa = 0m,
                Mora = 0m,
                DataVencimento = new DateTime(2022, 10, 10)
            };
        }

    }
}
