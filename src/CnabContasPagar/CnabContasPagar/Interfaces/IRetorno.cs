using CnabContasPagar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CNABContasPagar.Interfaces
{
    public interface IRetorno
    {
        Retorno LerArquivo(Stream ArquivoBase64);
    }
}