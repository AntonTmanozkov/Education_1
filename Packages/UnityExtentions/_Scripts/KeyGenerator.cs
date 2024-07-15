using System.Text.RegularExpressions;

namespace UnityExtentions
{
    public static class KeyGenerator
    {
        public static string CreateKeyInUpperCase(string input)
        {
            return Regex.Replace(input, "(\\B[A-Z])", "_$1").ToUpper().Replace(' ', '_');
        }
    }
}
