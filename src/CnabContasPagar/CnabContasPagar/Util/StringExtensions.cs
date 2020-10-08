
namespace CNABContasPagar
{
    public static class StringExtensions
    {
        public static string Slice(this string value, int start, int end)
        {
            start--;
            int len = end - start; ;

            return value.Substring(start, len);
        }
    }
}