using DuzceObs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuzceObs.Infrastructure
{
    public class DesignTimeDbContextFactory:IDesignTimeDbContextFactory<DuzceObsDbContext>
    {
        public DuzceObsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DuzceObsDbContext>();
            const string connectionString = "***";
            builder.UseSqlServer(connectionString);
            return new DuzceObsDbContext(builder.Options);
        }
    }
}
