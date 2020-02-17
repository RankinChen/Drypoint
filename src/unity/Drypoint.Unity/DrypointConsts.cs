using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity
{
    public class DrypointConsts
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public const string ConnectionStringName = "Default";

        /// <summary>
        /// 数据库连接字符串（PostgreSQL）
        /// </summary>
        public const string ConnectionStringName_PostgreSQL = "PostgreSQL";
        
        /// <summary>
        /// 接口前缀
        /// </summary>
        public const string ApiPrefix = "api/";


        /// <summary>
        /// 列表数据 分页 每页最多显示条数
        /// </summary>
        public const int MaxPageSize = 1000;

        /// <summary>
        /// 列表数据 每页条数
        /// </summary>
        public const int DefaultPageSize = 10;

        public const string AdminAPIGroupName = "admin";

        public const string AppAPIGroupName = "app";

        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";

        public const string CacheKey_TokenValidityKey = "token_validity_key";

        public static string CacheKey_UserIdentifier = "user_identifier";
    }

    /// <summary>
    /// 数据库种类
    /// </summary>
    public enum DBCategory { 

        /// <summary>
        /// MS SQL Server
        /// </summary>
        SQLServer=0,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL = 1,
    }
}
