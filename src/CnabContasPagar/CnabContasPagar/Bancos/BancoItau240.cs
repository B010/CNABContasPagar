using CnabContasPagar.Models;
using CnabContasPagar.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CnabContasPagar.Bancos
{
    class BancoItau240
    {
        public BancoItau240(Opcoes opcoes)
        {
            Opcoes = opcoes;
        }

        public Opcoes Opcoes { get; set; }

        public string MontarArquivo()
        {
            var b = new StringBuilder();

            Header(b);
                        
            

            return b.ToString();
        }

        public void Header(StringBuilder b)
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
    }
}
