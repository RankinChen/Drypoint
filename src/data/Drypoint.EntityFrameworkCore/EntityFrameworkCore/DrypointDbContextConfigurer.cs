using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Drypoint.EntityFrameworkCore.EntityFrameworkCore
{
    public static class DrypointDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<DrypointDbContext> builder, string connectionString,
            bool isUseRowNumber = true)
        {
            if (isUseRowNumber)
            {
                builder.UseSqlServer(connectionString, p => p.UseRowNumberForPaging());
            }
            else
            {
                builder.UseSqlServer(connectionString);
            }
        }

        public static void Configure(DbContextOptionsBuilder<DrypointDbContext> builder, DbConnection connection,
            bool isUseRowNumber = true)
        {
            if (isUseRowNumber)
            {
                builder.UseSqlServer(connection, p => p.UseRowNumberForPaging());
            }
            else
            {
                builder.UseSqlServer(connection);
            }
        }
    }
}
