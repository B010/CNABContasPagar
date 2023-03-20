using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CnabContasPagar.Util
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendTexto(this StringBuilder sb, int tamanho, string txt)
        {
            if (txt == null)
                txt = "";

            txt = Regex.Replace(txt.Normalize(NormalizationForm.FormD), "[^A-Za-z0-9| ]", string.Empty).ToUpper();

            if (txt.Length > tamanho)
                sb.Append(txt.Substring(0, tamanho));
            else
                sb.Append(txt.PadRight(tamanho, ' '));

            return sb;
        }

        public static StringBuilder AppendNumero(this StringBuilder sb, int tamanho, string valor)
        {
            if (valor == null)
                valor = "";

            valor = Regex.Replace(valor.Normalize(NormalizationForm.FormD), "[^0-9| ]", string.Empty).ToUpper();

            valor = valor.PadLeft(tamanho, '0');

            if (valor.Length > tamanho)
                throw new Exception($"o valor {valor} excede o tamanho máximo de {tamanho} caracteres");

            sb.Append(valor.PadLeft(tamanho, '0'));

            return sb;
        }

        public static StringBuilder AppendNumero(this StringBuilder sb, int tamanho, long valor)
        {
            var s = valor.ToString("D" + tamanho);

            if (s.Length > tamanho)
                throw new Exception($"o valor {s} excede o tamanho máximo de {tamanho} caracteres");

            sb.Append(s);

            return sb;
        }

        public static StringBuilder AppendData(this StringBuilder sb, DateTime data)
        {
            return sb.AppendData(data, "ddMMyyyy");
        }

        public static StringBuilder AppendData(this StringBuilder sb, DateTime? data)
        {
            if(data != null)
            {
               return sb.AppendData(Convert.ToDateTime(data), "ddMMyy");
                
            }
            else
            {
                return sb.Append("000000");
            }
        }

        public static StringBuilder AppendData(this StringBuilder sb, DateTime data, string format)
        {
            sb.Append(data.ToString(format));

            return sb;
        }

        public static StringBuilder AppendDinheiro(this StringBuilder sb, int tamanho, decimal? valor)
        {
            if (valor == null)
                valor = 0m;

            var s = valor.Value.ToString("F2");

            var txt = Regex.Replace(s, "[.,]", string.Empty).PadLeft(tamanho, '0');

            sb.Append(txt);

            return sb;
        }

       
    }
}