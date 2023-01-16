using CnabContasPagar.Retornos;
using CnabContasPagar.Models;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace CNABContasPagar.Testes.Ret
{
    public class DetalheATests
    {
        public DetalheATests()
        {
        }

        [Fact]
        public void ArquivoBradesco500()
        {
            using (StreamReader sr = new StreamReader("D:\\Projetos\\CNABContasPagar\\src\\CnabContasPagar\\CnabContasPagar\\ArquivoRetorno\\Bradesco500.RET"))
            {
                var retorno = new Retornos();

                var asyu9dha = retorno.LerArquivo(sr.BaseStream);

                var x = 10;
            }

        }

    }
}
