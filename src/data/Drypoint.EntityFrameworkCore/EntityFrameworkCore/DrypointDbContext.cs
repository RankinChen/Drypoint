using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drypoint.Model.Auditing;
using Drypoint.Model.Authorization;
using Drypoint.Model.Authorization.Roles;
using Drypoint.Model.Authorization.Users;
using Drypoint.Model.Common;
using Drypoint.Model.Configuration;
using Drypoint.Unity.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Drypoint.EntityFrameworkCore.EntityFrameworkCore
{
    public partial class DrypointDbContext : DbContext
    {
        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserLogin> UserLogins { get; set; }

        public virtual DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }

        public virtual DbSet<UserRole> UserRoles { get; set; }

        public virtual DbSet<UserClaim> UserClaims { get; set; }

        public virtual DbSet<PermissionSetting> Permissions { get; set; }

        public virtual DbSet<RolePermissionSetting> RolePermissions { get; set; }

        public virtual DbSet<UserPermissionSetting> UserPermissions { get; set; }

        public virtual DbSet<Setting> Settings { get; set; }

        public virtual DbSet<AuditLog> AuditLogs { get; set; }


        private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(DrypointDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);
        public DrypointDbContext(DbContextOptions<DrypointDbContext> options)
            : base(options)
        {
            InitDbContext();
        }
        private void InitDbContext()
        {
            //TODO 

        }

        #region override

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        public override int SaveChanges()
        {
            try
            {
                var result = base.SaveChanges();
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        #endregion

        #region function
        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }
            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDeleted;
                expression = expression == null ? softDeleteFilter : ExpressionCombiner.Combine(expression, softDeleteFilter);
            }

            return expression;
        }

        #endregion
    }
}
