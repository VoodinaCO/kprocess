using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kprocess.KL2.EFCore.Models
{
    public static class SqlServerModelBuilderExtensions
    {
        public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> builder, int precision, int scale) =>
            builder.HasColumnType($"decimal({precision},{scale})");

        public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> builder, int precision, int scale) =>
            builder.HasColumnType($"decimal({precision},{scale})");
    }
}
