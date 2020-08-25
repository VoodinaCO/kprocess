using System.Text;

namespace KProcess.Ksmed.Models
{
    public static class Extensions
    {
        public static string ToHashString(this byte[] hash)
        {
            var builder = new StringBuilder();
            foreach (byte hashed in hash)
                builder.AppendFormat("{0:X2}", hashed);
            return builder.ToString();
        }
    }
}
