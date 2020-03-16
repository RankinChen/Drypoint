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
        public static void Configure(DbContextOptionsBuilder<DrypointDbContext> builder, string connectionString, DBCategory dbCategory = DBCategory.SQLServer)
        {
            switch (dbCategory)
            {
                case DBCategory.SQLServer:
                    builder.UseSqlServer(connectionString);
                    break;
                case DBCategory.PostgreSQL:
                    builder.UseNpgsql(connectionString);
                    break;
            }

        }

        public static void Configure(DbContextOptionsBuilder<DrypointDbContext> builder, DbConnection connection, DBCategory dbCategory)
        {
            switch (dbCategory)
            {
                case DBCategory.SQLServer:
                    builder.UseSqlServer(connection);
                    break;
                case DBCategory.PostgreSQL:
                    builder.UseNpgsql(connection);
                    break;
            }

        }
    }
}
