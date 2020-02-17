using Drypoint.Unity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Drypoint.EntityFrameworkCore.EntityFrameworkCore
{
    public static class DrypointDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<DrypointDbContext> builder, string connectionString, DBCategory dbCategory=DBCategory.SQLServer,
            bool isUseRowNumber = true)
        {
            switch (dbCategory)
            {
                case DBCategory.SQLServer:
                    if (isUseRowNumber)
                    {
                        builder.UseSqlServer(connectionString, p => p.UseRowNumberForPaging());
                    }
                    else
                    {
                        builder.UseSqlServer(connectionString);
                    }
                    break;
                case DBCategory.PostgreSQL:
                        builder.UseNpgsql(connectionString);
                    break;
            }

        }

        public static void Configure(DbContextOptionsBuilder<DrypointDbContext> builder, DbConnection connection, DBCategory dbCategory,
            bool isUseRowNumber = true)
        {
            switch (dbCategory)
            {
                case DBCategory.SQLServer:
                    if (isUseRowNumber)
                    {
                        builder.UseSqlServer(connection, p => p.UseRowNumberForPaging());
                    }
                    else
                    {
                        builder.UseSqlServer(connection);
                    }
                    break;
                case DBCategory.PostgreSQL:
                    builder.UseNpgsql(connection);
                    break;
            }
            
        }
    }
}
