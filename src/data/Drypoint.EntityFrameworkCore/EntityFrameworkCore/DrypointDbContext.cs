using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Drypoint.EntityFrameworkCore.EntityFrameworkCore
{
    public partial class DrypointDbContext : DbContext
    {
        public DrypointDbContext(DbContextOptions<DrypointDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
