using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Data.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ToPagedQuery<T>(this IQueryable<T> query,int pageNumber, int pageSize)
        {
            return query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
    }
}
